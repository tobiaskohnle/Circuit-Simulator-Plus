using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
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

            Self = this;

            DrawGrid();
            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                LoadState(StorageUtil.Load(args[1]));
            else
                ContextGate = new ContextGate();

            Timer.Tick += Timer_Tick;
        }

        #region Properties
        public static MainWindow Self;

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
        public bool CreatingCable;

        public bool Saved = true;
        public bool SingleTicks;

        public string FileName = Constants.DefaultTitle;
        public string CurrentFilePath;

        public double CurrentScale;

        public Cable CreatedCable;
        public ConnectionNode CableOrigin;

        public Queue<ConnectionNode> TickedNodes = new Queue<ConnectionNode>();
        public DispatcherTimer Timer = new DispatcherTimer();

        public List<IClickable> ClickableObjects = new List<IClickable>();
        public List<IClickable> SelectedObjects = new List<IClickable>();

        public DropOutStack<StorageObject> UndoStack = new DropOutStack<StorageObject>(Constants.UndoBufferSize);
        public DropOutStack<StorageObject> RedoStack = new DropOutStack<StorageObject>(Constants.UndoBufferSize);

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
            SaveState("Added gate");
            gate.Add();
        }

        public void Tick(ConnectionNode node)
        {
            node.IsTicked = true;
            TickedNodes.Enqueue(node);
            if (SingleTicks == false)
                Timer.Start();
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
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is Gate)
                {
                    (obj as Gate).Remove();
                }
                else if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Clear();
                }
                else if (obj is CableSegment)
                {
                    (obj as CableSegment).Remove();
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
                SaveState("Toggled objects");
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
                SaveState("Changed type to " + type.Name);

                StorageObject storageObject = null;
                if (type == typeof(ContextGate))
                    storageObject = StorageUtil.Load(SelectFile());

                foreach (IClickable obj in SelectedObjects.ToList())
                {
                    if (obj is Gate)
                    {
                        var gate = obj as Gate;
                        Gate newGate = null;

                        if (type == typeof(ContextGate))
                            newGate = StorageConverter.ToGate(storageObject);
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

                        ContextGate.Context.Remove(gate);
                        ContextGate.Context.Add(newGate);

                        foreach (OutputNode outputNode in newGate.Output)
                            Tick(outputNode);

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
                SaveState("Renamed objects");
                var renameWindow = new Controls.RenameWindow(null) { Owner = this };

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
                ResetFile();
                ResetView();

                return true;
            }

            return false;
        }

        public void LoadState(StorageObject storageObject)
        {
            ResetFile();
            ContextGate = StorageConverter.ToGateTopLayer(storageObject);
            ContextGate.AddContext();
        }

        public void ResetFile()
        {
            ContextGate.RemoveContext();

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

        public void Open(string filePath)
        {
            if (New())
            {
                CurrentFilePath = filePath;

                if (Properties.Settings.Default.RecentFiles == null)
                {
                    Properties.Settings.Default.RecentFiles = new StringCollection();
                }
                Properties.Settings.Default.RecentFiles.Remove(filePath);
                Properties.Settings.Default.RecentFiles.Insert(0, filePath);
                Properties.Settings.Default.Save();

                CollectionViewSource.GetDefaultView(Properties.Settings.Default.RecentFiles).Refresh();

                LoadState(StorageUtil.Load(filePath));
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

        public void Undo()
        {
            if (AllowUndo)
            {
                StorageObject lastState = UndoStack.Pop();
                LoadState(lastState);
                RedoStack.Push(lastState);
            }
        }
        public void Redo()
        {
            if (AllowRedo)
            {
                StorageObject lastState = RedoStack.Pop();
                LoadState(lastState);
                UndoStack.Push(lastState);
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
                StorageUtil.Save(CurrentFilePath, StorageConverter.ToStorageObject(ContextGate));
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
                FileName = System.IO.Path.GetFileNameWithoutExtension(dialog.SafeFileName);
                StorageUtil.Save(CurrentFilePath, StorageConverter.ToStorageObject(ContextGate));
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
                PasteFromClipboard(LastCanvasPos);
            }
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
            BackgroundGridPen = new Pen(Brushes.LightGray, 0);
            var geometry = new RectangleGeometry(new Rect(0, 0, 1, 1));
            var drawing = new GeometryDrawing(Brushes.White, BackgroundGridPen, geometry);
            var brush = new DrawingBrush
            {
                Drawing = drawing,
                Viewport = new Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Stretch = Stretch.Fill
            };
            backgoundLayerCanvas.Background = brush;
            UpdateGrid();
        }
        public void UpdateGrid()
        {
            BackgroundGridPen.Thickness = Constants.LineWidth / CurrentScale / 2;
            backgoundLayerCanvas.Background.Transform = canvas.RenderTransform;
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

                double scale = Math.Min(screenSize.Width / bounds.Width, screenSize.Height / bounds.Height);

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

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, at.X, at.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);

            CurrentScale *= scale;
            UpdateGrid();
        }
        #endregion

        #region Misc
        public void SaveState(string message)
        {
            Console.WriteLine("SAVE STATE: " + message);
            Saved = false;
            UpdateTitle();
            UndoStack.Push(StorageConverter.ToStorageObject(ContextGate));
            RedoStack.Clear();
        }

        public void MoveObjects()
        {
            Vector completeMove = LastCanvasPos - LastCanvasClick;

            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is IMovable)
                    (obj as IMovable).Move(-completeMove);
            }

            var movedObjects = new List<IMovable>();

            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is IMovable)
                {
                    movedObjects.Add(obj as IMovable);
                }
            }

            var movedVectors = new List<Vector>(movedObjects.Count);

            bool anyMoved = false;

            for (int i = 0; i < movedObjects.Count; i++)
            {
                IMovable obj = movedObjects[i];

                Point pos = new Point();

                if (obj is Gate)
                {
                    pos = (obj as Gate).Position;
                }
                else if (obj is ConnectionNode)
                {
                    pos = (obj as ConnectionNode).Position;
                }
                else if (obj is CableSegment)
                {
                    pos = (obj as CableSegment).Parent.GetPoint((obj as CableSegment).Index);
                }

                Vector moved = new Vector();
                if (obj is CableSegment)
                {
                    moved = Round(pos + completeMove, 0.5) - pos;
                }
                else
                {
                    moved = Round(pos + completeMove) - pos;
                }

                if (moved.X != 0 || moved.Y != 0)
                {
                    anyMoved = true;
                }

                movedVectors.Add(moved);
            }

            if (anyMoved)
            {
                SaveState("Moved objects");
                for (int i = 0; i < movedObjects.Count; i++)
                {
                    movedObjects[i].Move(movedVectors[i]);
                }
            }
        }
        public void CreateCable()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                {
                    CreatingCable = true;

                    ConnectionNode startNode = LastClickedObject as ConnectionNode;

                    CableOrigin = startNode;
                    CreatedCable = new Cable(startNode);

                    return;
                }
            }
        }

        public void EmptyInput()
        {

        }
        public void AddInputToSelected()
        {
            if (AnySelected<Gate>())
            {
                SaveState("Added empty input");
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is Gate)
                    {
                        (obj as Gate).AddEmptyInputNode();
                    }
                }
            }
        }
        public void RemoveInputFromSelected()
        {
            if (AnySelected<InputNode>())
            {
                SaveState("Removed input");
                foreach (IClickable obj in SelectedObjects)
                {
                    if (obj is InputNode)
                    {
                        Gate owner = (obj as InputNode).Owner;
                        owner.RemoveInputNode(obj as InputNode);
                    }
                }
            }
        }
        public void InvertConnection()
        {
            if (AnySelected<ConnectionNode>())
            {
                SaveState("Inverted connectionNode");
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
            foreach (IClickable obj in SelectedObjects)
            {
                Gate gate = obj as Gate;
                if (gate == null)
                    continue;
                storeContext.Context.Add(gate);
                if (gate.Position.X < ltcorner.X)
                    ltcorner.X = gate.Position.X;
                if (gate.Position.Y < ltcorner.Y)
                    ltcorner.Y = gate.Position.Y;
            }
            var store = StorageConverter.ToStorageObject(storeContext);
            foreach (StorageObject innerStore in store.Context)
                innerStore.Position = (Point)(innerStore.Position - ltcorner);
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
            ContextGate storeContext = StorageConverter.ToGateTopLayer(store);
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
            return SelectedObjects.Exists(obj => obj.GetType() == typeof(T));
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

            IClickable nearestObject = FindNearestObjectAt(LastCanvasClick);

            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                MovingScreen = true;

                LastClickedObject = nearestObject;
            }
            else if (CreatingCable)
            {
                if (nearestObject is ConnectionNode && nearestObject is InputNode != CableOrigin is InputNode)
                {
                    var connectionNode = nearestObject as ConnectionNode;
                    CreatedCable.ConnectTo(connectionNode);

                    connectionNode.ConnectTo(CableOrigin);
                    Tick(connectionNode);
                    Tick(CableOrigin);

                    CreatingCable = false;
                }
                else
                {
                    CreatedCable.AddSegment(Round(LastCanvasClick, 0.5));
                }
            }
            else
            {
                LastClickedObject = nearestObject;

                if (LastClickedObject == null)
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
        }
        void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CreatingCable)
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

                if (MovingObjects)
                {
                    if (!LastClickedObject.IsSelected)
                    {
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

                if (MakingSelection)
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
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (ControlPressed == false)
                        {
                            DeselectAll();
                        }
                        bool islastClickedSelected = LastClickedObject.IsSelected;
                        if (islastClickedSelected == false)
                        {
                            Select(LastClickedObject);
                        }
                    }
                    else
                    {
                        if (ControlPressed == false && LastClickedObject.IsSelected == false)
                        {
                            DeselectAll();
                        }
                        Select(LastClickedObject);
                    }
                }
            }

            MakingSelection = false;
            MovingObjects = false;
            MovingScreen = false;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
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

        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAt(
                e.DeltaManipulation.Scale.X,
                e.DeltaManipulation.Scale.X,
                e.ManipulationOrigin.X,
                e.ManipulationOrigin.Y
            );
            canvas.RenderTransform = new MatrixTransform(matrix);

            matrix.Translate(
                e.DeltaManipulation.Translation.X - e.DeltaManipulation.Translation.X,
                e.DeltaManipulation.Translation.Y - e.DeltaManipulation.Translation.Y
            );
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
            string filepath = SelectFile();
            if (filepath == "")
                return;
            StorageObject store = StorageUtil.Load(filepath);
            if (store == null)
                return;
            Gate gate = StorageConverter.ToGate(store);
            if (gate.HasContext)
                TickAll((ContextGate)gate);
            Create(gate);
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

        void RisingEdge_Click(object sender, RoutedEventArgs e)
        {
            ToggleRisingEdge();
        }
        void MasterSlave_Click(object sender, RoutedEventArgs e)
        {
            ToggleMasterSlave();
        }
        void Centered_Click(object sender, RoutedEventArgs e)
        {
            ToggleCentered();
        }

        void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }
        void Open_Click(object sender, RoutedEventArgs e)
        {
            var filePath = SelectFile();
            if (filePath != "")
            {
                Open(filePath);
            }
        }
        void RecentFiles_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source != e.OriginalSource)
            {
                Open((e.OriginalSource as MenuItem).Header.ToString());
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

        void MainToolbar_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void MainToolbar_Unchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void ShowGrid_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void ShowGrid_Unchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void ResetView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
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
        void Resize_Click(object sender, RoutedEventArgs e)
        {
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleObjects();
        }
        void RemoveConnection_Click(object sender, RoutedEventArgs e)
        {
        }
        void AddInput_Click(object sender, RoutedEventArgs e)
        {
            AddInputToSelected();
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
        void Resize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<Gate>();
        }
        void AddInput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected<Gate>();
        }

        void Version_Click(object sender, RoutedEventArgs e)
        {
        }
        void GithubLink_Click(object sender, RoutedEventArgs e)
        {
        }

        void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadState(StorageConverter.ToStorageObject(ContextGate));
        }
        void SingleTicks_Checked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Single Ticks Enabled");
            SingleTicks = true;
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

        void ConnectParallel_Click(object sender, RoutedEventArgs e)
        {
            ConnectOppositeNodes();
        }
        void CreateCable_Click(object sender, RoutedEventArgs e)
        {
            CreateCable();
        }
        #endregion
    }
}