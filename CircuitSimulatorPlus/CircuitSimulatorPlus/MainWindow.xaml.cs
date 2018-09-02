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
        public MainWindow()
        {
            InitializeComponent();

            Self = this;

            DrawGrid();
            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                ContextGate = (ContextGate)StorageConverter.ToGateTopLayer(StorageUtil.Load(args[1]));
            else
                ContextGate = new ContextGate();
            RenderContext();

            UpdateClickableObjects();

            Timer.Tick += Timer_Tick;

            canvas.Children.Add(NewCable);
            Panel.SetZIndex(NewCable, 1);
        }

        #region Properties
        public static MainWindow Self;

        public Point LastMousePos;

        public Point LastWindowPos;
        public Point LastWindowClick;

        public Point LastCanvasPos;
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

        public Line NewCable = new Line
        {
            Stroke = Brushes.DarkTurquoise,
            StrokeThickness = Constants.LineWidth,
            StrokeDashArray = new DoubleCollection { Constants.DefaultGridSize / 2, Constants.DefaultGridSize / 2 }
        };

        public Queue<ConnectionNode> TickedNodes = new Queue<ConnectionNode>();
        public DispatcherTimer Timer = new DispatcherTimer();

        public List<IClickable> ClickableObjects = new List<IClickable>();
        public List<IClickable> SelectedObjects = new List<IClickable>();

        public DropOutStack<Command> UndoStack = new DropOutStack<Command>(Constants.UndoBufferSize);
        public DropOutStack<Command> RedoStack = new DropOutStack<Command>(Constants.UndoBufferSize);

        public List<Cable> Cables = new List<Cable>();
        public ContextGate ContextGate;
        public IClickable LastClickedObject;

        public Pen BackgroundGridPen;
        #endregion

        #region Object
        public void Add(Gate gate)
        {
            gate.Position = new Point(Math.Round(LastCanvasClick.X), Math.Round(LastCanvasClick.Y));

            //PerformAction(new CreateGateAction(contextGate, gate));

            gate.IsRendered = true;
            foreach (ConnectionNode node in gate.Input)
                node.IsRendered = true;
            foreach (ConnectionNode node in gate.Output)
                node.IsRendered = true;

            Select(gate);
            ClickableObjects.Add(gate);
            ContextGate.Context.Add(gate);
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

        public void Select(IClickable obj)
        {
            if (!obj.IsSelected)
            {
                SelectedObjects.Add(obj);
                obj.IsSelected = true;
            }
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
                    Remove(obj as Gate);
                }
                else if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Clear();
                }
            }
            DeselectAll();
        }
        public void Remove(Gate gate)
        {
            foreach (InputNode input in gate.Input)
                Remove(input);
            foreach (OutputNode output in gate.Output)
                Remove(output);
            gate.IsRendered = false;
            ClickableObjects.Remove(gate);
            ContextGate.Context.Remove(gate);
        }
        public void Remove(ConnectionNode connectionNode)
        {
            connectionNode.IsRendered = false;
            connectionNode.Clear();
            ClickableObjects.Remove(connectionNode);
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
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is InputSwitch)
                {

                    InputSwitch inputSwitch = obj as InputSwitch;
                    inputSwitch.State = !inputSwitch.State;
                    Tick(inputSwitch.Output[0]);
                    //inputSwitch.Renderer.OnPositionChanged();
                }
                if (obj is ConnectionNode)
                {
                    ConnectionNode connectionNode = obj as ConnectionNode;
                    connectionNode.Invert();
                    Tick(connectionNode);
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
            StorageObject storageObject = null;
            if (type == typeof(ContextGate))
                storageObject = StorageUtil.Load(SelectFile());
            PerformCommand(new ChangeTypeCommand(type,SelectedObjects));
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
        #endregion

        #region IO
        public void New()
        {
            SavePrompt();

            foreach (IClickable obj in ClickableObjects)
            {
                if (obj is Gate)
                    (obj as Gate).IsRendered = false;
                else if (obj is ConnectionNode)
                    (obj as ConnectionNode).IsRendered = false;
            }

            ContextGate = new ContextGate();
            UpdateClickableObjects();
        }

        public string SelectFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = Constants.FileFilter;
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public void Open(string filePath)
        {
            CurrentFilePath = filePath;

            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
            }
            Properties.Settings.Default.RecentFiles.Remove(filePath);
            Properties.Settings.Default.RecentFiles.Insert(0, filePath);

            CollectionViewSource.GetDefaultView(Properties.Settings.Default.RecentFiles).Refresh();

            ContextGate = (ContextGate)StorageConverter.ToGateTopLayer(StorageUtil.Load(filePath));
            RenderContext();
            UpdateClickableObjects();
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
                Command lastAction = UndoStack.Pop();
                lastAction.Undo();
                RedoStack.Push(lastAction);
            }
        }
        public void Redo()
        {
            if (AllowRedo)
            {
                Command lastAction = RedoStack.Pop();
                lastAction.Redo();
                UndoStack.Push(lastAction);
            }
        }

        public void Save()
        {
            if (CurrentFilePath != null)
                StorageUtil.Save(CurrentFilePath, StorageConverter.ToStorageObject(ContextGate));
            else
                SaveAs();
            Saved = true;
            UpdateTitle();
        }
        public void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = FileName;
            dialog.DefaultExt = Constants.FileExtention;
            dialog.Filter = Constants.FileFilter;
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
            if (AnySelected)
            {

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
        public void PerformCommand(Command command)
        {
            Saved = false;
            UpdateTitle();
            command.Redo();
            UndoStack.Push(command);
            RedoStack.Clear();
        }
        public void UpdateClickableObjects()
        {
            ClickableObjects.Clear();
            foreach (Gate gate in ContextGate.Context)
            {
                ClickableObjects.Add(gate);
                foreach (InputNode input in gate.Input)
                    ClickableObjects.Add(input);
                foreach (OutputNode output in gate.Output)
                    ClickableObjects.Add(output);
            }
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

            if (movedObjects.Count > 0)
            {
                completeMove.X = Math.Round(completeMove.X);
                completeMove.Y = Math.Round(completeMove.Y);

                if (completeMove.X != 0 || completeMove.Y != 0)
                {
                    PerformCommand(new MoveCommand(movedObjects, completeMove));
                }
            }
        }
        public void CreateCable()
        {
            NewCable.Visibility = Visibility.Collapsed;
            var startNode = LastClickedObject as ConnectionNode;

            IClickable clickedObject = FindNearestObjectAt(LastCanvasPos);

            if (clickedObject is ConnectionNode)
            {
                var endNode = clickedObject as ConnectionNode;
                if (startNode is InputNode != endNode is InputNode && startNode != endNode)
                {
                    if (startNode is InputNode)
                    {
                        var temp = startNode;
                        startNode = endNode;
                        endNode = temp;
                    }

                    Console.WriteLine($"Created cable");
                    startNode.ConnectTo(endNode);
                    Tick(endNode);

                    //var cable = new Cable(endNode as InputNode, startNode as OutputNode);
                }
            }
        }

        public void EmptyInput()
        {

        }
        public void AddInputToSelected()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is Gate)
                {
                    PerformCommand(new AddInputCommand(obj as Gate));
                }
            }
        }
        public void RemoveInputFromSelected()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is InputNode)
                {
                    PerformCommand(new DeleteInputCommand(obj as InputNode));
                }
            }
        }
        public void InvertConnection()
        {
            foreach (IClickable obj in SelectedObjects)
                if (obj is ConnectionNode)
                {
                    ConnectionNode connectionNode = obj as ConnectionNode;
                    PerformCommand(new InvertConnectionCommand(SelectedObjects));
                    Tick(connectionNode);
                }
        }
        public void RenderContext()
        {
            foreach (Gate gate in ContextGate.Context)
            {
                gate.IsRendered = true;
                foreach (ConnectionNode node in gate.Input)
                {
                    node.IsRendered = true;
                }
                foreach (ConnectionNode node in gate.Output)
                {
                    node.IsRendered = true;
                }
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

        public bool AnySelected
        {
            get
            {
                return SelectedObjects.Count > 0;
            }
        }
        public bool AnyGateSelected
        {
            get
            {
                return SelectedObjects.Exists(obj => obj is Gate);
            }
        }
        public bool AnyConnectionSelected
        {
            get
            {
                return SelectedObjects.Exists(obj => obj is ConnectionNode);
            }
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
            else
            {
                if (LastClickedObject == null)
                {
                    MakingSelection = true;
                }
                else if (LastClickedObject is IMovable)
                {
                    MovingObjects = true;
                }
                else if (LastClickedObject is ConnectionNode)
                {
                    NewCable.Visibility = Visibility.Visible;
                    NewCable.X1 = NewCable.X2 = (LastClickedObject as ConnectionNode).Position.X;
                    NewCable.Y1 = NewCable.Y2 = (LastClickedObject as ConnectionNode).Position.Y;
                    CreatingCable = true;
                }
                else
                {
                    MakingSelection = true;
                }
            }

            if (MakingSelection)
            {
                Canvas.SetLeft(selectVisual, LastWindowClick.X);
                Canvas.SetTop(selectVisual, LastWindowClick.Y);
                selectVisual.Visibility = Visibility.Visible;
            }
        }
        void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleObjects();
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

                if (CreatingCable)
                {
                    NewCable.X2 = LastCanvasPos.X;
                    NewCable.Y2 = LastCanvasPos.Y;

                    NewCable.StrokeDashOffset = Constants.DefaultGridSize * -0.25 * Math.Sqrt(
                        Math.Pow(NewCable.X1 - NewCable.X2, 2) + Math.Pow(NewCable.Y1 - NewCable.Y2, 2)
                    );
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

                if (CreatingCable)
                {
                    CreateCable();
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
            CreatingCable = false;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.T)
            {
                TickQueue();
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

        void Timer_Tick(object sender, EventArgs e)
        {
            TickQueue();
        }
        #endregion

        #region UI Event Handlers
        void CreateInputSwitch(object sender, RoutedEventArgs e)
        {
            Add(new InputSwitch());
        }
        void CreateOutputLight(object sender, RoutedEventArgs e)
        {
            Add(new OutputLight());
        }
        void CreateAndGate(object sender, RoutedEventArgs e)
        {
            Add(new AndGate());
        }
        void CreateOrGate(object sender, RoutedEventArgs e)
        {
            Add(new OrGate());
        }
        void CreateNotGate(object sender, RoutedEventArgs e)
        {
            var newGate = new NopGate();
            Add(newGate);
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
        }
        void CreateSegmentDisplay(object sender, RoutedEventArgs e)
        {
            Add(new SegmentDisplay());
        }

        void Import_Click(object sender, RoutedEventArgs e)
        {
            Gate gate = StorageConverter.ToGate(StorageUtil.Load(SelectFile()));
            Add(gate);
            UpdateClickableObjects();
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
                New();
                Open(filePath);
            }
        }
        void RecentFiles_Click(object sender, RoutedEventArgs e)
        {
            Open((e.OriginalSource as MenuItem).Header.ToString());
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
            e.CanExecute = AnySelected;
        }
        void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DataOnClipboard;
        }
        void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void SelectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClickableObjects.Count > 0;
        }
        void DeselectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Format_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
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
            ZoomIntoView(SelectedObjects);
        }

        void ZoomSelection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }

        void InvertConnection_Click(object sender, RoutedEventArgs e)
        {
            InvertConnection();
        }
        void Rename_Click(object sender, RoutedEventArgs e)
        {
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
            e.CanExecute = AnyConnectionSelected;
        }
        void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Resize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyGateSelected;
        }
        void AddInput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyGateSelected;
        }

        void Version_Click(object sender, RoutedEventArgs e)
        {
        }
        void GithubLink_Click(object sender, RoutedEventArgs e)
        {
        }

        void Reload_Click(object sender, RoutedEventArgs e)
        {
            var storageObject = StorageConverter.ToStorageObject(ContextGate);
            New();
            ContextGate = (ContextGate)StorageConverter.ToGateTopLayer(storageObject);
            RenderContext();
            UpdateClickableObjects();
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
            foreach (Gate gate in ContextGate.Context)
            {
                foreach (InputNode inputNode in gate.Input)
                    Tick(inputNode);
                foreach (OutputNode outputNode in gate.Output)
                    Tick(outputNode);
            }
        }
        #endregion

        private void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAt(e.DeltaManipulation.Scale.X,
            e.DeltaManipulation.Scale.X,
            e.ManipulationOrigin.X,
            e.ManipulationOrigin.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);

            matrix.Translate(e.DeltaManipulation.Translation.X - e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y - e.DeltaManipulation.Translation.Y);

        }

        private void Window_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 9.7 * 96.0 / (1000.0 * 1000.0);
            e.ExpansionBehavior.DesiredDeceleration = 0.1 * 95 / (1000.0 * 1000.0);
        }

        private void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }
    }
}
