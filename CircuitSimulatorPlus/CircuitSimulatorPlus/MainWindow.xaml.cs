using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Threading;

namespace CircuitSimulatorPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            canvas.SnapsToDevicePixels = true;

            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                contextGate = StorageConverter.ToGate(Storage.Load(args[1]));
            else
                contextGate = new Gate();
            foreach (Gate gate in contextGate.Context)
                gate.Renderer.Render();
            List<Gate> createdGates = contextGate.Context;

            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += TimerTick;
            DrawGrid(ScaleFactor);
        }

        #region Constants
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileFormat;
        public const string DefaultTitle = "untitled";
        public const string FileFormat = "tici";
        public const string Unsaved = "\u2022";
        public const double MinDistMouseMoved = 4;
        public const double DefaultGridSize = 20;
        public const double ScaleFactor = 0.9;
        public const double LineWidth = 0.1;
        public const int UndoBufferSize = 32;
        #endregion

        #region Properties
        Point lastMousePos;

        Point lastWindowPos;
        Point lastWindowClick;

        Point lastCanvasPos;
        Point lastCanvasClick;

        bool makingSelection;
        bool movingObjects;
        bool movingScreen;
        bool drawingCable;
        bool mouseMoved;

        bool saved = true;
        bool singleTicks;

        string fileName = DefaultTitle;
        string currentFilePath;

        Queue<ConnectionNode> tickedNodes = new Queue<ConnectionNode>();
        DispatcherTimer timer = new DispatcherTimer();

        List<IClickable> clickableObjects = new List<IClickable>();
        List<IClickable> selected = new List<IClickable>();

        Stack<Action> undoStack = new Stack<Action>();
        Stack<Action> redoStack = new Stack<Action>();

        List<Cable> cables = new List<Cable>();
        Gate contextGate = new Gate();
        IClickable lastClickedObject;

        //Rectangle selectVisual = new Rectangle {
        //    Fill = new SolidColorBrush(Color.FromArgb(127, 63, 63, 255)),
        //    Stroke = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255)),
        //    StrokeThickness = 1,
        //};
        #endregion

        #region Gates
        public void Add(Gate gate)
        {
            contextGate.Context.Add(gate);
            clickableObjects.Add(gate);
            foreach (InputNode input in gate.Input)
                Add(input);
            foreach (OutputNode output in gate.Output)
                Add(output);
        }
        public void Add(ConnectionNode connectionNode)
        {
            clickableObjects.Add(connectionNode);
        }
        public void CreateGate(Gate gate, int amtInputs, int amtOutputs, Point at)
        {
            contextGate.Context.Add(gate);
            gate.Renderer = new GateRenderer(canvas, gate, OnGateInputClicked, OnGateOutputClicked);

            for (int i = 0; i < amtInputs; i++)
                gate.Input.Add(new InputNode(gate));
            for (int i = 0; i < amtOutputs; i++)
                gate.Output.Add(new OutputNode(gate));

            gate.Position = new Point(Math.Round(at.X), Math.Round(at.Y));
            gate.Renderer.Render();

            //PerformAction(new CreateGateAction(contextGate, gate));

            Select(gate);
            Add(gate);
        }

        public void Tick(ConnectionNode node)
        {
            tickedNodes.Enqueue(node);
            if (singleTicks == false)
                timer.Start();
        }
        public void TickQueue()
        {
            List<ConnectionNode> copy = tickedNodes.ToList();
            tickedNodes.Clear();
            foreach (ConnectionNode ticked in copy)
                ticked.Tick(tickedNodes);
            if (tickedNodes.Count == 0)
                timer.Stop();
        }
        public void TimerTick(object sender, EventArgs e)
        {
            TickQueue();
        }

        public void Select(IClickable obj)
        {
            if (!obj.IsSelected)
            {
                selected.Add(obj);
                obj.IsSelected = true;
            }
        }
        public void Deselect(IClickable obj)
        {
            if (obj.IsSelected)
            {
                selected.Remove(obj);
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
            return clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(rect));
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
            foreach (IClickable obj in clickableObjects)
                Select(obj);
        }
        public void DeselectAll()
        {
            foreach (IClickable obj in selected)
                obj.IsSelected = false;
            selected.Clear();
        }

        public void DeleteSelected()
        {

        }
        public void Delete(Gate gate)
        {
            foreach (InputNode input in gate.Input)
                input.Clear();
            foreach (OutputNode output in gate.Output)
                output.Clear();
            gate.Renderer.Unrender();
            contextGate.Context.Remove(gate);
        }

        public IClickable FindNearestObjectAt(Point pos)
        {
            IClickable nearest = null;
            double best = Double.PositiveInfinity;

            var objectsAtPos = clickableObjects.Where(obj => obj.Hitbox.IncludesPos(pos));
            foreach (IClickable obj in objectsAtPos)
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

        public void CreateAndGate(object sender, RoutedEventArgs e)
        {
            CreateGate(new Gate(Gate.GateType.And), 2, 1, lastCanvasClick);
        }
        #endregion

        #region Visuals
        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(DefaultGridSize, DefaultGridSize);
            canvas.RenderTransform = new MatrixTransform(matrix);
        }
        public void UpdateTitle()
        {
            Title = $"{fileName}{(saved ? "" : " " + Unsaved)} - {WindowTitle}";
        }
        public void DrawGrid(double scale)
        {
            double linewidth = LineWidth / scale;
            VisualBrush brush = new VisualBrush
            {
                Viewport = new Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Stretch = Stretch.Fill
            };

            Grid grid = new Grid { Width = 992, Height = 648 };
            grid.Children.Add(new Rectangle
            {
                Width = 1,
                Height = linewidth,
                Fill = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            });

            grid.Children.Add(new Rectangle
            {
                Height = 1,
                Width = linewidth,
                Fill = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            });

            brush.Visual = grid;
            canvas.Background = brush;
        }
        #endregion

        #region Misc
        public void PerformAction(Action action)
        {
            saved = false;
            UpdateTitle();
            action.Redo();
        }
        public bool SavePrompt()
        {
            //if (!saved)
            //{
            //    switch (MessageBox.Show(
            //        "Would you like to save your changes?",
            //        "Save?",
            //        MessageBoxButton.YesNoCancel
            //    ))
            //    {
            //    case MessageBoxResult.Yes:
            //        Save(); return saved;
            //    case MessageBoxResult.No:
            //        return true;
            //    case MessageBoxResult.Cancel:
            //        return false;
            //    }
            //}
            return true;
        }
        #endregion

        #region Util
        public bool AllowUndo
        {
            get {
                return undoStack.Count > 0;
            }
        }
        public bool AllowRedo
        {
            get {
                return redoStack.Count > 0;
            }
        }
        public bool AnySelected
        {
            get {
                return selected.Count > 0;
            }
        }
        public bool ControlPressed
        {
            get {
                return (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            }
        }
        public bool DataOnClipboard
        {
            get {
                return Clipboard.ContainsData(FileFormat);
            }
        }
        #endregion

        #region Events
        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();

            lastCanvasClick = e.GetPosition(canvas);
            lastWindowClick = e.GetPosition(this);

            lastClickedObject = FindNearestObjectAt(lastCanvasClick);

            mouseMoved = false;

            makingSelection = true;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                Canvas.SetLeft(selectVisual, lastWindowClick.X);
                Canvas.SetTop(selectVisual, lastWindowClick.Y);
                selectVisual.Visibility = Visibility.Visible;
                movingScreen = true;
            }
            else
            {

            }
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentWindowPos = e.GetPosition(this);
            Point currentCanvasPos = e.GetPosition(canvas);

            Vector windowMoved = currentWindowPos - lastWindowPos;
            Vector canvasMoved = e.GetPosition(canvas) - lastCanvasPos;

            if ((lastMousePos - lastWindowClick).LengthSquared >= MinDistMouseMoved * MinDistMouseMoved)
            {
                mouseMoved = true;
            }

            if (mouseMoved)
            {
                if (movingScreen)
                {
                    Matrix matrix = canvas.RenderTransform.Value;
                    matrix.Translate(windowMoved.X, windowMoved.Y);
                    canvas.RenderTransform = new MatrixTransform(matrix);
                }

                if (movingObjects)
                {
                    foreach (IClickable obj in selected)
                        if (obj is Gate)
                            (obj as Gate).Move(canvasMoved);
                }

                if (makingSelection)
                {
                    var innerRect = new Rect(lastCanvasClick, lastCanvasPos);
                    var outerRect = new Rect(lastCanvasClick, currentCanvasPos);
                    var innerObjects = clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(innerRect));
                    var outerObjects = clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(outerRect));

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

                    if (currentCanvasPos.X < lastCanvasClick.X)
                    {
                        Canvas.SetLeft(selectVisual, currentCanvasPos.X);
                        selectVisual.Width = lastCanvasClick.X - currentCanvasPos.X;
                    }
                    else
                    {
                        selectVisual.Width = currentCanvasPos.X - lastCanvasClick.X;
                    }
                    if (currentCanvasPos.Y < lastCanvasClick.Y)
                    {
                        Canvas.SetTop(selectVisual, currentCanvasPos.Y);
                        selectVisual.Height = lastCanvasClick.Y - currentCanvasPos.Y;
                    }
                    else
                        selectVisual.Height = currentCanvasPos.Y - lastCanvasClick.Y;
                }

                lastWindowPos = currentWindowPos;
                lastCanvasPos = e.GetPosition(canvas);
            }

            lastMousePos = currentWindowPos;
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            if (mouseMoved)
            {
                if (movingObjects)
                {
                    Vector completeMove = lastCanvasPos - lastCanvasClick;
                    foreach (IClickable obj in selected)
                        if (obj is Gate)
                            (obj as Gate).Move(-completeMove);
                    if (completeMove.LengthSquared > 0.25)
                        PerformAction(new MoveObjectAction(selected,
                            new Vector(Math.Round(completeMove.X), Math.Round(completeMove.Y))));
                }
            }

            if (makingSelection)
            {
                var selectedRect = new Rect(lastCanvasClick, lastCanvasPos);
                if (ControlPressed)
                {
                    SwitchSelected(lastClickedObject);
                }
                else if (lastClickedObject == null)
                {
                    DeselectAll();
                }

                selectVisual.Width = 0;
                selectVisual.Height = 0;
                selectVisual.Visibility = Visibility.Collapsed;
            }

            //if (drawingCable)
            //{
            //    Cable lastcable = cables.Last();
            //    Point pos = e.GetPosition(canvas);
            //    pos.X = Math.Round(pos.X);
            //    pos.Y = Math.Round(pos.Y);
            //    lastcable.AddPoint(pos);
            //}

            //if (drawingCable && lastClickedObject is ConnectionNode)
            //{
            //    drawingCable = false;
            //}

            makingSelection = false;
            movingObjects = false;
            movingScreen = false;
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
            double scale = e.Delta > 0 ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, currentPos.X, currentPos.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);
            DrawGrid(scale);
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && singleTicks)
                TickQueue();
        }
        void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void Window_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = mouseMoved;
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        void NewFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in contextGate.Context)
                gate.Renderer.Unrender();
            foreach (Cable cable in cables)
                cable.Renderer.Unrender();
        }
        void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            NewFile_Click(sender, e);
            var dialog = new OpenFileDialog();
            dialog.Filter = "Circuit File (.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                contextGate = StorageConverter.ToGate(Storage.Load(dialog.FileName));
                foreach (Gate gate in contextGate.Context)
                {
                    gate.Renderer = new GateRenderer(canvas, gate, OnGateInputClicked, OnGateOutputClicked);
                    gate.Renderer.Render();
                    for (int i = 0; i < gate.Output.Count; i++)
                    {
                        OutputNode node = gate.Output[i];
                        Point p1 = new Point();
                        p1.X = gate.Position.X + 3 + 1;
                        p1.Y = gate.Position.Y + (double)4 * (1 + 2 * i) / (2 * gate.Output.Count);
                        foreach (InputNode inNode in node.NextConnectedTo)
                        {
                            Gate inGate = inNode.Owner;
                            int index = 0;
                            for (int j = 0; j < inGate.Input.Count; j++)
                            {
                                if (inGate.Input[j] == inNode)
                                {
                                    index = j;
                                    break;
                                }
                            }
                            Point p2 = new Point();
                            p2.X = inGate.Position.X - 1;
                            p2.Y = inGate.Position.Y + (double)4 * (1 + 2 * index) / (2 * inGate.Input.Count);
                            Cable cable = new Cable();
                            cable.Output = node;
                            cable.Input = inNode;
                            cable.Renderer = new CableRenderer(canvas, cable);
                            cable.AddPoint(p1, true);
                            cable.AddPoint(p2, true);
                            cables.Add(cable);
                        }
                    }
                }
            }
        }
        void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Circuit";
            dialog.DefaultExt = ".json";
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                Storage.Save(path, StorageConverter.ToStorageObject(contextGate));
            }
        }
        void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {

        }

        void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUndo)
            {
                Action lastAction = undoStack.Pop();
                lastAction.Undo();
                redoStack.Push(lastAction);
            }
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (AllowRedo)
            {
                Action lastAction = redoStack.Pop();
                lastAction.Redo();
                undoStack.Push(lastAction);
            }
        }
        void Copy_Click(object sender, RoutedEventArgs e)
        {

        }
        void Cut_Click(object sender, RoutedEventArgs e)
        {

        }
        void Paste_Click(object sender, RoutedEventArgs e)
        {

        }
        void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnySelected)
            {
                //PerformAction(new DeleteGateAction(contextGate, selected));
                DeleteSelected();
            }
        }

        void DefaultView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }
        void ZoomIn_Click(object sender, RoutedEventArgs e)
        {

        }
        void ZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }

        void NewGate_Click(object sender, RoutedEventArgs e)
        {

        }
        void Rename_Click(object sender, RoutedEventArgs e)
        {

        }
        void ResizeGate_Click(object sender, RoutedEventArgs e)
        {

        }
        void EmptyInput_Click(object sender, RoutedEventArgs e)
        {

        }
        void TrimInputs_Click(object sender, RoutedEventArgs e)
        {

        }
        void InvertConnection_Click(object sender, RoutedEventArgs e)
        {

        }
        void SelectAll_Click(object sender, RoutedEventArgs e)
        {

        }
        void ContextGate_Click(object sender, RoutedEventArgs e)
        {
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
        }

        void SingleTicks_Click(object sender, RoutedEventArgs e)
        {
            singleTicks = !singleTicks;
            MessageBox.Show($"Single Ticks {(singleTicks ? "Enabled (Advance by pressing 'T')" : "Disabled")}");
            if (singleTicks == false && tickedNodes.Count > 0)
                timer.Start();
        }
        void TickAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in contextGate.Context)
            {
                foreach (InputNode inputNode in gate.Input)
                    Tick(inputNode);
                foreach (OutputNode outputNode in gate.Output)
                    Tick(outputNode);
            }
        }

        void OnGateOutputClicked(object sender, EventArgs e)
        {
            Gate gate = (Gate)sender;
            int index = ((IndexEventArgs)e).Index;

            if (!drawingCable)
            {
                Point point = new Point();
                point.X = gate.Position.X + 3 + 1;
                point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Output.Count);
                Cable cable = new Cable();
                cables.Add(cable);
                cable.Renderer = new CableRenderer(canvas, cable);
                cable.Output = gate.Output[index];
                drawingCable = true;
            }
        }
        void OnGateInputClicked(object sender, EventArgs e)
        {
            Gate gate = (Gate)sender;
            int index = ((IndexEventArgs)e).Index;

            if (drawingCable)
            {
                Point point = new Point();
                point.X = gate.Position.X - 1;
                point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Input.Count);
                Cable lastcable = cables.Last();
                lastcable.AddPoint(point, true);
                lastcable.Input = gate.Input[index];
                lastcable.Output.ConnectTo(lastcable.Input);
                Tick(lastcable.Input);
                drawingCable = false;
            }
        }
        #endregion
    }
}
