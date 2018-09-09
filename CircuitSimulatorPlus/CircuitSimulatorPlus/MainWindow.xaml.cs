using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace CircuitSimulatorPlus
{
    public partial class MainWindow : Window
    {
        public List<WeakReference<IClickable>> refs = new List<WeakReference<IClickable>>();

        public MainWindow()
        {
            InitializeComponent();
            Focus();
            LoadContextGates();
            LoadTheme();

            Self = this;

            showGrid.IsChecked = true;
            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                LoadState(StorageUtil.Load(args[1]));
            else
                ContextGate = new ContextGate();

            ContextGate.AddContext();
            Timer.Tick += Timer_Tick;
        }

        #region Properties
        public static MainWindow Self;

        public ITheme Theme = new ClassicTheme();

        public Point LastMousePos;

        public Point LastWindowPos;
        public Point LastWindowClick;

        public event Action OnLastCanvasPosChanged;
        Point lastCanvasPos;
        public Point LastCanvasPos
        {
            get
            {
                return lastCanvasPos;
            }
            set
            {
                lastCanvasPos = value;
                OnLastCanvasPosChanged?.Invoke();
            }
        }
        public Point LastCanvasClick;

        public bool MakingSelection;
        public bool MovingObjects;
        public bool MovingScreen;
        public bool MouseMoved;
        public bool CableCreated;
        public bool CreatingCable;

        public bool Saved = true;
        public bool SingleTicks;

        public bool ShowGrid;

        public string FileName = Constants.DefaultTitle;
        public string CurrentFilePath;

        public double CurrentScale;

        public Cable CreatedCable;
        public ConnectionNode CableOrigin;

        public Queue<ConnectionNode> TickedNodes = new Queue<ConnectionNode>();
        public DispatcherTimer Timer = new DispatcherTimer();

        public List<IClickable> ClickableObjects = new List<IClickable>();
        public List<IClickable> SelectedObjects = new List<IClickable>();

        public DropOutStack<SerializedGate> UndoStack = new DropOutStack<SerializedGate>(Constants.UndoBufferSize);
        public DropOutStack<SerializedGate> RedoStack = new DropOutStack<SerializedGate>(Constants.UndoBufferSize);

        public List<Cable> Cables = new List<Cable>();
        public ContextGate ContextGate;
        public IClickable LastClickedObject;

        public Pen BackgroundGridPen;
        #endregion

        #region Object
        public void Create(Gate gate)
        {
            gate.Position = Round(LastCanvasClick);
            gate.CreateDefaultConnectionNodes();
            SaveState();
            gate.Add();
        }

        public void Tick(ConnectionNode node)
        {
            if (SingleTicks)
                node.IsTicked = true;
            TickedNodes.Enqueue(node);
            if (SingleTicks == false)
                Timer.Start();
        }
        public void Tick(Gate gate)
        {
            foreach (OutputNode outputNode in gate.Output)
            {
                Tick(outputNode);
            }
        }

        public void TickQueue()
        {
            if (TickedNodes.Count > 0)
            {
                Console.WriteLine($"Tick {TickedNodes.Count} Nodes");
            }
            else
            {
                Console.WriteLine($"No Nodes to Tick");
                return;
            }

            List<ConnectionNode> tickedNodesCopy = TickedNodes.ToList();
            TickedNodes.Clear();
            foreach (ConnectionNode ticked in tickedNodesCopy)
            {
                ticked.IsTicked = false;
                ticked.Tick();
            }
            if (TickedNodes.Count == 0)
                Timer.Stop();

            Console.WriteLine($"{TickedNodes.Count} Nodes Enqueued");

            if (SingleTicks)
                foreach (var node in TickedNodes)
                    node.IsTicked = true;
        }
        public void TickAll(ContextGate contextGate)
        {
            foreach (Gate gate in contextGate.Context)
            {
                foreach (InputNode inputNode in gate.Input)
                    Tick(inputNode);
                foreach (OutputNode outputNode in gate.Output)
                    Tick(outputNode);
                if (gate.HasContext)
                    TickAll((ContextGate)gate);
            }
        }

        public void Select(IClickable obj)
        {
            if (!obj.IsSelected)
            {
                SelectedObjects.Add(obj);
            }
            obj.IsSelected = true;
        }
        public void Deselect(IClickable obj)
        {
            if (obj.IsSelected)
            {
                SelectedObjects.Remove(obj);
                obj.IsSelected = false;
            }
        }
        public void SwitchSelected(IClickable obj)
        {
            if (obj.IsSelected)
            {
                Deselect(obj);
            }
            else
            {
                Select(obj);
            }
        }

        public IEnumerable<IClickable> GetObjectsIn(Rect rect)
        {
            return ClickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(rect));
        }
        public void SelectAllIn(Rect rect)
        {
            foreach (IClickable obj in GetObjectsIn(rect))
                Select(obj);
        }
        public void SwitchSelectionIn(Rect rect)
        {
            foreach (IClickable obj in GetObjectsIn(rect))
                SwitchSelected(obj);
        }
        public void SelectAll()
        {
            foreach (IClickable obj in ClickableObjects)
                Select(obj);
        }
        public void DeselectAll()
        {
            foreach (IClickable obj in SelectedObjects)
                obj.IsSelected = false;
            SelectedObjects.Clear();
        }

        public void Delete()
        {
            if (CableCreated)
            {
                SaveState();
                CancelCable();
            }
            else
            {
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is Gate)
                    {
                        SaveState();
                        (obj as Gate).Remove();
                    }
                    else if (obj is ConnectionNode)
                    {
                        if (!(obj as ConnectionNode).IsEmpty)
                            SaveState();
                        (obj as ConnectionNode).Clear();
                    }
                    else if (obj is CableSegment)
                    {
                        SaveState();
                        var cableSegment = obj as CableSegment;
                        if (cableSegment.Index == 0)
                        {
                            cableSegment.Parent.Clear();
                        }
                        else if (cableSegment.Index == cableSegment.Parent.Segments.Count - 1)
                        {
                            cableSegment.Parent.Clear();
                        }
                        else
                        {
                            cableSegment.Remove();
                        }
                    }
                }
            }
            DeselectAll();
        }

        public IClickable FindNearestObjectAt(Point pos)
        {
            IClickable nearest = null;
            double best = Double.PositiveInfinity;

            foreach (IClickable obj in ClickableObjects.Where(obj => obj.Hitbox.IncludesPos(pos)))
            {
                double dist = obj.Hitbox.DistanceTo(pos);
                if (dist < best)
                {
                    nearest = obj;
                    best = dist;
                }
            }

            return nearest;
        }

        public void ToggleObjects()
        {
            if (AnySelected<ConnectionNode>() || AnySelected<InputSwitch>())
            {
                SaveState();
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is ConnectionNode)
                    {
                        (obj as ConnectionNode).Invert();
                        Tick(obj as ConnectionNode);
                    }
                    else if (obj is InputSwitch)
                    {
                        (obj as InputSwitch).Toggle();
                        Tick((obj as InputSwitch).Output[0]);
                    }
                }
            }
        }
        public void ConnectAllNodes()
        {
            foreach (OutputNode outputNode in SelectedObjects.Where(obj => obj is OutputNode))
                foreach (InputNode inputNode in SelectedObjects.Where(obj => obj is InputNode))
                    outputNode.ConnectTo(inputNode);
        }
        public void ConnectOppositeNodes()
        {
            var outputs = SelectedObjects.Where(obj => obj is OutputNode).ToList();
            var inputs = SelectedObjects.Where(obj => obj is InputNode).ToList();

            for (int i = 0; i < Math.Min(inputs.Count, outputs.Count); i++)
                (outputs[i] as OutputNode).ConnectTo(inputs[i] as InputNode);
        }

        public void ChangeType(Type type)
        {
            if (AnySelected<Gate>())
            {
                SaveState();

                SerializedGate storageObject = null;
                if (type == typeof(ContextGate))
                    storageObject = StorageUtil.Load(SelectFile());

                foreach (IClickable obj in SelectedObjects.ToList())
                {
                    if (obj is Gate)
                    {
                        var gate = obj as Gate;
                        Gate newGate = null;

                        if (type == typeof(ContextGate))
                            newGate = GateSerializer.Deserialize(storageObject);
                        else if (type == typeof(InputSwitch))
                            newGate = new InputSwitch();
                        else if (type == typeof(OutputLight))
                            newGate = new OutputLight();
                        else if (type == typeof(AndGate))
                            newGate = new AndGate();
                        else if (type == typeof(OrGate))
                            newGate = new OrGate();
                        else if (type == typeof(NopGate))
                            newGate = new NopGate();
                        else if (type == typeof(SegmentDisplay))
                            newGate = new SegmentDisplay();

                        newGate.CopyFrom(gate);

                        newGate.UpdateAmtConnectionNodes();

                        gate.Remove(false);
                        newGate.Add(false);

                        Tick(newGate);

                        Deselect(gate);
                        Select(newGate);
                    }
                }
            }
        }

        public void ToggleRisingEdge()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is InputNode)
                {
                    (obj as InputNode).IsRisingEdge = !(obj as InputNode).IsRisingEdge;
                }
            }
        }
        public void ToggleMasterSlave()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is OutputNode)
                {
                    (obj as OutputNode).IsMasterSlave = !(obj as OutputNode).IsMasterSlave;
                }
            }
        }
        public void ToggleCentered()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).IsCentered = !(obj as ConnectionNode).IsCentered;
                }
            }
        }

        public void SplitSegments()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is CableSegment)
                {
                    (obj as CableSegment).SplitSegment();
                }
            }
        }

        public void Rename()
        {
            if (AnySelected<Gate>() || AnySelected<ConnectionNode>())
            {
                SaveState();

                string oldName = null;
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is Gate)
                    {
                        oldName = (obj as Gate).Name;
                    }
                    else if (obj is ConnectionNode)
                    {
                        oldName = (obj as ConnectionNode).Name;
                    }
                    break;
                }

                var renameWindow = new Controls.RenameWindow(oldName) { Owner = this };

                if (renameWindow.ShowDialog() == true)
                {
                    foreach (IClickable obj in SelectedObjects)
                    {
                        if (obj is Gate)
                        {
                            (obj as Gate).Name = renameWindow.Name;
                        }
                        else if (obj is ConnectionNode)
                        {
                            (obj as ConnectionNode).Name = renameWindow.Name;
                        }
                    }
                }
            }
        }
        #endregion

        #region IO
        public bool New()
        {
            if (SavePrompt())
            {
                CurrentFilePath = null;
                FileName = Constants.DefaultTitle;
                Saved = true;
                UpdateTitle();

                ResetFile();
                ResetView();

                return true;
            }

            return false;
        }

        public void LoadState(SerializedGate storageObject)
        {
            ResetFile();
            ContextGate = GateSerializer.DeserializeTopLayer(storageObject, Cables);
            ContextGate.AddContext();
        }

        public void ResetFile()
        {
            ContextGate.RemoveContext();
            foreach (Cable cable in Cables.ToList())
                cable.Remove();
            Cables.Clear();

            ContextGate = new ContextGate();

            DeselectAll();

            TickedNodes.Clear();
            Timer.Stop();
        }

        public string SelectFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = Constants.FileFilter
            };
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public void Open()
        {
            if (SavePrompt())
            {
                Open(SelectFile());
            }
        }
        public void Open(string filePath)
        {
            if (filePath != "")
            {
                ResetFile();
                ResetView();

                CurrentFilePath = filePath;
                FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                Saved = true;
                UpdateTitle();

                AddToRecentFiles(filePath);

                LoadState(StorageUtil.Load(filePath));
                ContextGate.AddContext();
                TickAll(ContextGate);
            }
        }

        public bool SavePrompt()
        {
            if (!Saved)
            {
                switch (MessageBox.Show(
                    "Would you like to save your changes?",
                    "Save?",
                    MessageBoxButton.YesNoCancel
                ))
                {
                case MessageBoxResult.Yes:
                    Save();
                    return Saved;
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
                }
            }
            return true;
        }

        public void Import()
        {
            Import(SelectFile());
        }
        public void Import(string filePath)
        {
            if (filePath == "")
                return;
            SerializedGate store = StorageUtil.Load(filePath);
            if (store == null)
                return;
            Gate gate = GateSerializer.Deserialize(store);
            if (gate.HasContext)
                TickAll((ContextGate)gate);
            Create(gate);
        }

        public void Undo()
        {
            if (AllowUndo)
            {
                RedoStack.Push(GateSerializer.SerializeTopLayer(ContextGate, Cables));
                SerializedGate lastState = UndoStack.Pop();
                LoadState(lastState);
            }
        }
        public void Redo()
        {
            if (AllowRedo)
            {
                UndoStack.Push(GateSerializer.SerializeTopLayer(ContextGate, Cables));
                SerializedGate lastState = RedoStack.Pop();
                LoadState(lastState);
            }
        }

        public void Save()
        {
            if (CurrentFilePath == null)
            {
                SaveAs();
            }
            else
            {
                StorageUtil.Save(CurrentFilePath, GateSerializer.SerializeTopLayer(ContextGate, Cables));
                Saved = true;
            }
            UpdateTitle();
        }
        public void SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                FileName = FileName,
                DefaultExt = Constants.FileExtention,
                Filter = Constants.FileFilter
            };
            if (dialog.ShowDialog() == true)
            {
                CurrentFilePath = dialog.FileName;
                AddToRecentFiles(CurrentFilePath);
                FileName = System.IO.Path.GetFileNameWithoutExtension(dialog.SafeFileName);
                StorageUtil.Save(CurrentFilePath, GateSerializer.SerializeTopLayer(ContextGate, Cables));
                Saved = true;
                UpdateTitle();
            }
        }

        public void Copy()
        {
            if (AnySelected())
            {
                CopyToClipboard();
            }
        }
        public void Cut()
        {
            Copy();
            Delete();
        }
        public void Paste()
        {
            if (DataOnClipboard)
            {
                DeselectAll();
                SaveState();
                PasteFromClipboard(LastCanvasPos);
            }
        }

        public void AddToRecentFiles(string filePath)
        {
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
            }
            Properties.Settings.Default.RecentFiles.Remove(filePath);
            Properties.Settings.Default.RecentFiles.Insert(0, filePath);
            Properties.Settings.Default.Save();

            CollectionViewSource.GetDefaultView(Properties.Settings.Default.RecentFiles).Refresh();
        }

        public void LoadContextGates()
        {
            //string path = Properties.Settings.Default.ContextGatePath;
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "context_gates");

            if (Directory.Exists(path))
            {
                LoadContextGates(contxtMenu_contextGate, path);
            }
            else
            {
                Console.WriteLine($"\"{path}\" is not a valid directory");
            }
        }

        public void LoadContextGates(MenuItem parent, string path)
        {
            foreach (string dir in Directory.EnumerateDirectories(path))
            {
                var subMenu = new MenuItem();
                subMenu.Header = System.IO.Path.GetFileName(dir);

                parent.Items.Add(subMenu);
                LoadContextGates(subMenu, dir);
            }
            foreach (string dir in Directory.EnumerateFiles(path))
            {
                if (System.IO.Path.GetExtension(dir) == Constants.FileExtention)
                {
                    var menuItem = new MenuItem();
                    menuItem.Header = System.IO.Path.GetFileNameWithoutExtension(dir);
                    menuItem.Click += new RoutedEventHandler((sender, e) => Import(dir));
                    parent.Items.Add(menuItem);
                }
            }
        }
        
        public void LoadTheme()
        {
            string theme = Properties.Settings.Default.Theme;

            if (theme == typeof(ClassicTheme).Name)
                Theme = new ClassicTheme();
            else if (theme == typeof(DarkTheme).Name)
                Theme = new DarkTheme();
            else
                Console.WriteLine($"Unknown theme \"{theme}\"");
        }
        #endregion

        #region Visuals
        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(Constants.DefaultGridSize, Constants.DefaultGridSize);
            canvas.RenderTransform = new MatrixTransform(matrix);
            CurrentScale = 1;
            UpdateGrid();
        }
        public void UpdateTitle()
        {
            Title = $"{FileName}{(Saved ? "" : " \u2022")} - {Constants.WindowTitle}";
        }

        public void DrawGrid()
        {
            BackgroundGridPen = new Pen(Theme.GridLine, 0);
            var geometry = new RectangleGeometry(new Rect(0, 0, 1, 1));
            var drawing = new GeometryDrawing(Theme.Background, BackgroundGridPen, geometry);
            var brush = new DrawingBrush
            {
                Drawing = drawing,
                Viewport = new Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Stretch = Stretch.Fill
            };
            if (ShowGrid)
            {
                backgoundLayerCanvas.Background = brush;
            }
            else
            {
                backgoundLayerCanvas.Background = null;
                Background = Theme.Background;
            }
            UpdateGrid();
        }
        public void UpdateGrid()
        {
            if (ShowGrid)
            {
                BackgroundGridPen.Thickness = Constants.LineWidth / CurrentScale / 2;
                backgoundLayerCanvas.Background.Transform = canvas.RenderTransform;
            }
        }

        public void ZoomIntoView(List<IClickable> objects)
        {
            if (objects.Count > 0)
            {
                double minX = Double.PositiveInfinity;
                double minY = Double.PositiveInfinity;
                double maxX = Double.NegativeInfinity;
                double maxY = Double.NegativeInfinity;

                foreach (IClickable obj in objects)
                {
                    Rect rect = obj.Hitbox.RectBounds;
                    if (minX > rect.Left)
                        minX = rect.Left;
                    if (minY > rect.Top)
                        minY = rect.Top;
                    if (maxX < rect.Right)
                        maxX = rect.Right;
                    if (maxY < rect.Bottom)
                        maxY = rect.Bottom;
                }

                var bounds = new Rect(minX, minY, maxX - minX, maxY - minY);
                double centerX = minX / 2 + maxX / 2;
                double centerY = minY / 2 + maxY / 2;

                var screenSize = new Rect(
                    TranslatePoint(new Point(0, 0), canvas),
                    TranslatePoint(new Point(canvas.ActualWidth, canvas.ActualHeight), canvas)
                );

                double scale = Math.Min(screenSize.Width / bounds.Width, screenSize.Height / bounds.Height) * 0.85;

                Matrix matrix = canvas.RenderTransform.Value;

                if (scale < 1)
                {
                    matrix.ScaleAtPrepend(scale, scale, CanvasCenter.X, CanvasCenter.Y);
                    CurrentScale *= scale;
                }

                matrix.TranslatePrepend(CanvasCenter.X - centerX, CanvasCenter.Y - centerY);
                canvas.RenderTransform = new MatrixTransform(matrix);

                UpdateGrid();
            }
        }
        public void Zoom(bool zoomIn, Point at)
        {
            double scale = zoomIn ? Constants.ScaleFactor : 1 / Constants.ScaleFactor;
            Zoom(scale, at);
        }
        public void Zoom(double scale, Point at)
        {
            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, at.X, at.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);

            CurrentScale *= scale;
            UpdateGrid();
        }
        #endregion

        #region Misc
        public void SaveState()
        {
            Saved = false;
            UpdateTitle();
            UndoStack.Push(GateSerializer.SerializeTopLayer(ContextGate, Cables));
            RedoStack.Clear();
        }

        public void MoveObjects()
        {
            Vector completeMove = LastCanvasPos - LastCanvasClick;

            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is IMovable)
                {
                    (obj as IMovable).Move(-completeMove);
                }
            }

            var movedObjects = new List<IMovable>();

            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is IMovable)
                {
                    movedObjects.Add(obj as IMovable);
                }
            }

            var points = new List<Point>(movedObjects.Count);

            bool snapToUnit = false;

            for (int i = 0; i < movedObjects.Count; i++)
            {
                IMovable obj = movedObjects[i];

                Point pos = new Point();

                if (obj is Gate)
                {
                    snapToUnit = true;
                    pos = (obj as Gate).Position;
                }
                else if (obj is CableSegment)
                {
                    pos = (obj as CableSegment).Parent.GetPoint((obj as CableSegment).Index);
                }

                points.Add(pos);
            }

            var vectors = new List<Vector>(movedObjects.Count);

            bool anyMoved = false;

            for (int i = 0; i < movedObjects.Count; i++)
            {
                var vector = new Vector();
                if (movedObjects[i] is CableSegment && !snapToUnit)
                {
                    vector = Round(points[i] + completeMove, 0.5) - points[i];
                }
                else
                {
                    vector = Round(points[i] + completeMove) - points[i];
                }

                if (vector.X != 0 || vector.Y != 0)
                {
                    anyMoved = true;
                }

                vectors.Add(vector);
            }

            if (anyMoved)
            {
                SaveState();
                for (int i = 0; i < movedObjects.Count; i++)
                {
                    movedObjects[i].Move(vectors[i]);
                }
            }
        }
        public void CreateCable()
        {
            if (!CableCreated)
            {
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is ConnectionNode)
                    {
                        CableCreated = true;

                        ConnectionNode startNode = LastClickedObject as ConnectionNode;

                        CableOrigin = startNode;
                        CreatedCable = new Cable(startNode);

                        return;
                    }
                }
            }
        }
        public void CompleteCable()
        {
            SaveState();

            var connectionNode = LastClickedObject as ConnectionNode;

            CreatedCable.ConnectTo(connectionNode);

            connectionNode.ConnectTo(CableOrigin);
            Tick(connectionNode);
            Tick(CableOrigin);

            Cables.Add(CreatedCable);

            CableCreated = false;
        }
        public void CancelCable()
        {
            CreatedCable.Remove();
            CreatedCable = null;
            CableOrigin = null;
            CableCreated = false;
        }

        public void EmptyInput()
        {

        }

        public void AddInputToSelected()
        {
            if (AnySelected<Gate>())
            {
                SaveState();
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is Gate)
                    {
                        (obj as Gate).AddEmptyInputNode();
                        Tick(obj as Gate);
                    }
                }
            }
        }

        public void RemoveInputFromSelected()
        {
            bool stateSaved = false;
            if (AnySelected<InputNode>())
            {
                SaveState();
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is InputNode)
                    {
                        Gate owner = (obj as InputNode).Owner;
                        if (!stateSaved)
                        {
                            SaveState();
                            stateSaved = true;
                        }
                        owner.RemoveInputNode(obj as InputNode);
                        Tick(owner);
                    }
                }
            }
            else if (AnySelected<Gate>())
            {
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is Gate)
                    {
                        Gate gate = obj as Gate;
                        if (gate.Input.Count > 0)
                        {
                            if (!stateSaved)
                            {
                                SaveState();
                                stateSaved = true;
                            }
                            gate.RemoveInputNode();
                        }
                        Tick(gate);
                    }
                }
            }
        }

        public void InvertConnection()
        {
            if (AnySelected<ConnectionNode>())
            {
                SaveState();
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is ConnectionNode)
                    {
                        (obj as ConnectionNode).Invert();
                        Tick(obj as ConnectionNode);
                    }
                }
            }
        }
        public void CopyToClipboard()
        {
            var storeContext = new ContextGate();
            Point ltcorner = new Point(double.PositiveInfinity, double.PositiveInfinity);
            var allnodes = new List<ConnectionNode>();
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                    allnodes.Add((ConnectionNode)obj);
                Gate gate = obj as Gate;
                if (gate == null)
                    continue;
                storeContext.Context.Add(gate);
                if (gate.Position.X < ltcorner.X)
                    ltcorner.X = gate.Position.X;
                if (gate.Position.Y < ltcorner.Y)
                    ltcorner.Y = gate.Position.Y;
            }
            var cables = new List<Cable>();
            foreach (Cable cable in Cables)
            {
                if (allnodes.Contains(cable.StartNode) && allnodes.Contains(cable.EndNode))
                    cables.Add(cable);
            }
            var store = GateSerializer.SerializeTopLayer(storeContext, cables);
            foreach (SerializedGate innerStore in store.Context)
                innerStore.Position = (Point)(innerStore.Position - ltcorner);
            foreach (SerializedGate.Cable cable in store.Cables)
            {
                for (int i = 0; i < cable.Points.Count; i++)
                    cable.Points[i] = cable.Points[i] - (Vector)ltcorner;
            }
            string storeText = StorageUtil.CreateText(store);
            Clipboard.SetData(Constants.FileExtention, storeText);
        }
        public void PasteFromClipboard(Point at)
        {
            at.X = Math.Round(at.X);
            at.Y = Math.Round(at.Y);
            string text = (string)Clipboard.GetData(Constants.FileExtention);
            var store = StorageUtil.LoadString(text);
            if (store == null)
                return;
            var cables = new List<Cable>();
            ContextGate storeContext = GateSerializer.DeserializeTopLayer(store, cables);
            foreach (Cable cable in cables)
            {
                for (int i = 0; i < cable.Points.Count; i++)
                {
                    cable.Points[i] = at + (Vector)cable.Points[i];
                }
                Cables.Add(cable);
            }
            foreach (Gate gate in storeContext.Context)
            {
                gate.Position = at + (Vector)gate.Position;
                gate.Add();
                Select(gate);
            }
        }
        #endregion

        #region Util
        public bool AllowUndo
        {
            get
            {
                return UndoStack.Count > 0;
            }
        }
        public bool AllowRedo
        {
            get
            {
                return RedoStack.Count > 0;
            }
        }

        public bool AnySelected()
        {
            return SelectedObjects.Count > 0;
        }
        public bool AnySelected<T>()
        {
            return SelectedObjects.Exists(obj => obj.GetType() == typeof(T) || obj.GetType().IsSubclassOf(typeof(T)));
        }

        public bool ControlPressed
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            }
        }
        public bool ShiftPressed
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) > 0;
            }
        }

        public bool DataOnClipboard
        {
            get
            {
                return Clipboard.ContainsData(Constants.FileExtention);
            }
        }

        public Point CanvasCenter
        {
            get
            {
                Matrix matrix = canvas.RenderTransform.Value;
                matrix.Invert();
                return matrix.Transform(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2));
            }
        }

        public Point Round(Point point, double a = 1)
        {
            return new Point(Math.Round(point.X / a) * a, Math.Round(point.Y / a) * a);
        }
        #endregion

        #region Window Event Handlers
        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    return;
                }
            }
            else if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    return;
                }
            }

            CaptureMouse();

            LastCanvasClick = e.GetPosition(canvas);
            LastWindowClick = e.GetPosition(this);

            MouseMoved = false;

            LastClickedObject = FindNearestObjectAt(LastCanvasClick);

            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                MovingScreen = true;
            }
            else if (ShiftPressed)
            {
                MakingSelection = true;
            }
            else if (CableCreated)
            {
                if (LastClickedObject is ConnectionNode && LastClickedObject is InputNode != CableOrigin is InputNode)
                {
                    CompleteCable();
                }
                else
                {
                    CreatedCable.AddSegment(LastCanvasClick);
                }
            }
            else if (LastClickedObject is ConnectionNode)
            {
                CreatingCable = true;
            }
            else if (LastClickedObject == null)
            {
                MakingSelection = true;
            }
            else if (LastClickedObject is IMovable)
            {
                MovingObjects = true;
            }
            else
            {
                MakingSelection = true;
            }
        }
        void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CableCreated)
            {
                CreatedCable.AutoComplete();
            }
            else
            {
                ToggleObjects();

                foreach (IClickable obj in SelectedObjects.ToList())
                {
                    if (obj is CableSegment)
                    {
                        foreach (CableSegment segment in (obj as CableSegment).Parent.Segments)
                        {
                            Select(segment);
                        }
                    }
                }
            }
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentWindowPos = e.GetPosition(this);
            Point currentCanvasPos = e.GetPosition(canvas);

            Vector windowMoved = currentWindowPos - LastWindowPos;
            Vector canvasMoved = currentCanvasPos - LastCanvasPos;

            if (MouseMoved)
            {
                if (MovingScreen)
                {
                    Matrix matrix = canvas.RenderTransform.Value;
                    matrix.Translate(windowMoved.X, windowMoved.Y);
                    canvas.RenderTransform = new MatrixTransform(matrix);
                }
                else if (CreatingCable)
                {
                    Select(LastClickedObject);
                    CreateCable();
                    CreatingCable = false;
                }
                else if (MovingObjects)
                {
                    if (!LastClickedObject.IsSelected)
                    {
                        if (!ControlPressed)
                            DeselectAll();
                        Select(LastClickedObject);
                    }
                    if (SelectedObjects.Count == 0)
                    {
                        Select(LastClickedObject);
                    }
                    foreach (IClickable obj in SelectedObjects)
                    {
                        if (obj is IMovable)
                            (obj as IMovable).Move(canvasMoved);
                    }
                }
                else if (MakingSelection)
                {
                    var innerRect = new Rect(LastCanvasClick, LastCanvasPos);
                    var outerRect = new Rect(LastCanvasClick, currentCanvasPos);
                    var innerObjects = ClickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(innerRect));
                    var outerObjects = ClickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(outerRect));

                    foreach (IClickable obj in outerObjects.Except(innerObjects))
                    {
                        if (ControlPressed)
                            SwitchSelected(obj);
                        else
                            Select(obj);
                    }

                    foreach (IClickable obj in innerObjects.Except(outerObjects))
                    {
                        if (ControlPressed)
                            SwitchSelected(obj);
                        else
                            Deselect(obj);
                    }

                    Canvas.SetLeft(selectVisual, LastWindowClick.X);
                    Canvas.SetTop(selectVisual, LastWindowClick.Y);
                    selectVisual.Visibility = Visibility.Visible;

                    if (currentWindowPos.X < LastWindowClick.X)
                    {
                        Canvas.SetLeft(selectVisual, currentWindowPos.X);
                        selectVisual.Width = LastWindowClick.X - currentWindowPos.X;
                    }
                    else
                    {
                        selectVisual.Width = currentWindowPos.X - LastWindowClick.X;
                    }
                    if (currentWindowPos.Y < LastWindowClick.Y)
                    {
                        Canvas.SetTop(selectVisual, currentWindowPos.Y);
                        selectVisual.Height = LastWindowClick.Y - currentWindowPos.Y;
                    }
                    else
                    {
                        selectVisual.Height = currentWindowPos.Y - LastWindowClick.Y;
                    }
                }

                LastWindowPos = currentWindowPos;
                LastCanvasPos = e.GetPosition(canvas);
            }
            else if ((LastMousePos - LastWindowClick).LengthSquared >= Constants.MinPxMouseMoved * Constants.MinPxMouseMoved)
            {
                if (MakingSelection && ControlPressed == false)
                {
                    DeselectAll();
                    SelectAllIn(new Rect(LastCanvasClick, LastCanvasPos));
                }
                MouseMoved = true;
            }

            LastMousePos = currentWindowPos;

            UpdateGrid();
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    return;
                }
            }
            else if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    return;
                }
            }

            ReleaseMouseCapture();

            if (MouseMoved)
            {
                if (MakingSelection)
                {
                    selectVisual.Width = selectVisual.Height = 0;
                    selectVisual.Visibility = Visibility.Collapsed;
                }

                if (MovingObjects)
                {
                    MoveObjects();
                }
            }
            else
            {
                if (LastClickedObject == null)
                {
                    DeselectAll();
                }
                else
                {
                    if (!ControlPressed && !LastClickedObject.IsSelected)
                    {
                        DeselectAll();
                    }

                    if (ControlPressed)
                    {
                        SwitchSelected(LastClickedObject);
                    }
                    else
                    {
                        Select(LastClickedObject);
                    }
                }
            }

            MakingSelection = false;
            MovingObjects = false;
            MovingScreen = false;
            CreatingCable = false;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (CableCreated)
                {
                    CancelCable();
                }
                DeselectAll();
            }

            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.T)
            {
                TickQueue();
            }

            if (e.Key == Key.S)
            {
                SplitSegments();
            }

            if (e.Key == Key.C)
            {
                CreateCable();
            }
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom(e.Delta > 0, e.GetPosition(canvas));
        }

        void Window_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = MouseMoved;
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !SavePrompt();
        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Point at = new Point(e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            Zoom(e.DeltaManipulation.Scale.X, at);
        }
        void Window_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 9.7 * 96 / (1000 * 1000);
            e.ExpansionBehavior.DesiredDeceleration = 0.1 * 95 / (1000 * 1000);
        }
        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            TickQueue();
        }
        #endregion

        #region UI Event Handlers
        void CreateInputSwitch(object sender, RoutedEventArgs e)
        {
            Create(new InputSwitch());
        }
        void CreateOutputLight(object sender, RoutedEventArgs e)
        {
            Create(new OutputLight());
        }
        void CreateAndGate(object sender, RoutedEventArgs e)
        {
            Create(new AndGate());
        }
        void CreateOrGate(object sender, RoutedEventArgs e)
        {
            Create(new OrGate());
        }
        void CreateNotGate(object sender, RoutedEventArgs e)
        {
            var newGate = new NopGate();
            Create(newGate);
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
        }
        void CreateSegmentDisplay(object sender, RoutedEventArgs e)
        {
            Create(new SegmentDisplay());
        }

        void Import_Click(object sender, RoutedEventArgs e)
        {
            Import();
        }

        void ChangeTypeContext(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(ContextGate));
        }
        void ChangeTypeInputSwitch(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(InputSwitch));
        }
        void ChangeTypeOutputLight(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(OutputLight));
        }
        void ChangeTypeAndGate(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(AndGate));
        }
        void ChangeTypeOrGate(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(OrGate));
        }
        void ChangeTypeNotGate(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(NopGate));
        }
        void ChangeTypeSegmentDisplay(object sender, RoutedEventArgs e)
        {
            ChangeType(typeof(SegmentDisplay));
        }

        void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }
        void Open_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }
        void RecentFiles_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source != e.OriginalSource)
            {
                if (New())
                {
                    Open((e.OriginalSource as MenuItem).Header.ToString());
                }
            }
        }
        void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFilePath != null)
            {
                Process.Start(System.IO.Path.GetDirectoryName(CurrentFilePath));
            }
        }
        void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }
        void Print_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OpenFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentFilePath != null;
        }

        void Undo_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            Redo();
        }
        void Cut_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CUT");
            Cut();
        }
        void Copy_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("COPY");
            Copy();
        }
        void Paste_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("PASTE");
            Paste();
        }
        void Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }
        void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }
        void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            DeselectAll();
        }
        void Format_Click(object sender, RoutedEventArgs e)
        {
        }

        void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AllowUndo;
        }
        void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AllowRedo;
        }
        void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }
        void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }
        void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DataOnClipboard;
        }
        void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }
        void SelectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClickableObjects.Count > 0;
        }
        void DeselectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }
        void Format_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }

        void ShowGrid_Checked(object sender, RoutedEventArgs e)
        {
            ShowGrid = true;
            DrawGrid();
        }
        void ShowGrid_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowGrid = false;
            DrawGrid();
        }
        void ResetView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }
        void ClassicTheme_Click(object sender, RoutedEventArgs e)
        {
            Theme = new ClassicTheme();
            Properties.Settings.Default.Theme = typeof(ClassicTheme).Name;
            Properties.Settings.Default.Save();
        }
        void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Theme = new DarkTheme();
            Properties.Settings.Default.Theme = typeof(DarkTheme).Name;
            Properties.Settings.Default.Save();
        }
        void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Zoom(true, CanvasCenter);
        }
        void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Zoom(false, CanvasCenter);
        }
        void ZoomSelection_Click(object sender, RoutedEventArgs e)
        {
            if (AnySelected())
            {
                ZoomIntoView(SelectedObjects);
            }
            else
            {
                ZoomIntoView(ClickableObjects);
            }
        }

        void InvertConnection_Click(object sender, RoutedEventArgs e)
        {
            InvertConnection();
        }
        void Rename_Click(object sender, RoutedEventArgs e)
        {
            Rename();
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleObjects();
        }
        void ToggleRisingEdge_Click(object sender, RoutedEventArgs e)
        {
            ToggleRisingEdge();
        }
        void AddInput_Click(object sender, RoutedEventArgs e)
        {
            AddInputToSelected();
        }
        void RemoveInput_Click(object sender, RoutedEventArgs e)
        {
            RemoveInputFromSelected();
        }
        void TrimInput_Click(object sender, RoutedEventArgs e)
        {
        }

        void InvertConnection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<ConnectionNode>();
        }
        void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected();
        }
        void ToggleButton_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<InputSwitch>();
        }
        void ToggleRisingEdge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<InputNode>();
        }
        void Resize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<Gate>();
        }
        void AddInput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<Gate>();
        }
        void RemoveInput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<InputNode>() || AnySelected<Gate>();
        }

        void Version_Click(object sender, RoutedEventArgs e)
        {
        }
        void GithubLink_Click(object sender, RoutedEventArgs e)
        {
        }

        void Reload_Click(object sender, RoutedEventArgs e)
        {
            SerializedGate state = GateSerializer.SerializeTopLayer(ContextGate, Cables);
            ResetFile();
            LoadState(state);
        }
        void SingleTicks_Checked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Single Ticks Enabled");
            SingleTicks = true;
            Timer.Stop();
        }
        void SingleTicks_Unchecked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Single Ticks Disabled");
            SingleTicks = false;
            if (TickedNodes.Count > 0)
                Timer.Start();
        }
        void TickAll_Click(object sender, RoutedEventArgs e)
        {
            TickAll(ContextGate);
        }
        void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }
        void LogReferences_Click(object sender, RoutedEventArgs e)
        {
            foreach (var refr in refs)
            {
                IClickable target = null;
                bool alive = refr.TryGetTarget(out target);
                Console.WriteLine((target?.GetType().Name ?? "null") + ": " + (alive ? "Alive" : "Destroyed"));
            }

            Console.WriteLine();
        }
        #endregion
    }
}