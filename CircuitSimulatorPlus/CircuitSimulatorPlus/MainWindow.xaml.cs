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

            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                contextGate = (ContextGate)StorageConverter.ToGate(Storage.Load(args[1]));
            else
                contextGate = new ContextGate();

            UpdateClickableObjects();

            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += TimerTick;

            DrawGrid();

            var cable = new Cable();
            var segment0 = new CableSegment();
            var segment1 = new CableSegment();
            var segment2 = new CableSegment();
            var joint0 = new CableJoint();
            var joint1 = new CableJoint();
            var joint2 = new CableJoint();
            var joint3 = new CableJoint();

            segment0.A = joint0;
            segment1.A = segment0.B = joint1;
            segment2.A = segment1.B = joint2;
            segment2.B = joint3;

            joint0.After = joint1.Before = segment0;
            joint1.After = joint2.Before = segment1;
            joint2.After = joint3.Before = segment2;

            segment0.Renderer = new CableSegmentRenderer(canvas, segment0);
            segment1.Renderer = new CableSegmentRenderer(canvas, segment1);
            segment2.Renderer = new CableSegmentRenderer(canvas, segment2);
            joint0.Renderer = new CableJointRenderer(canvas, joint0);
            joint1.Renderer = new CableJointRenderer(canvas, joint1);
            joint2.Renderer = new CableJointRenderer(canvas, joint2);
            joint3.Renderer = new CableJointRenderer(canvas, joint3);

            joint0.Move(new Vector(5, 5));
            joint1.Move(new Vector(10, 5));
            joint2.Move(new Vector(10, 10));
            joint3.Move(new Vector(15, 10));

            segment0.UpdateHitbox();
            segment1.UpdateHitbox();
            segment2.UpdateHitbox();

            segment0.Renderer.OnPositionChanged();
            segment1.Renderer.OnPositionChanged();
            segment2.Renderer.OnPositionChanged();

            clickableObjects.Add(segment0);
            clickableObjects.Add(segment1);
            clickableObjects.Add(segment2);
            clickableObjects.Add(joint0);
            clickableObjects.Add(joint1);
            clickableObjects.Add(joint2);
            clickableObjects.Add(joint3);
        }

        #region Constants
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileExtention;
        public const string DefaultTitle = "untitled";
        public const string FileExtention = ".tici";
        public const double MinPxMouseMoved = 5;
        public const double DefaultGridSize = 20;
        public const double ScaleFactor = 0.9;
        public const double LineWidth = Unit / 10;
        public const double LineRadius = Unit / 20;
        public const double InversionDotDiameter = Unit / 2;
        public const double InversionDotRadius = Unit / 4;
        public const double CableJointSize = Unit / 3;
        public const double Unit = 1;
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
        bool mouseMoved;
        bool creatingCable;

        bool saved = true;
        bool singleTicks;

        string fileName = DefaultTitle;
        string currentFilePath;

        double currentScale;

        Queue<ConnectionNode> tickedNodes = new Queue<ConnectionNode>();
        DispatcherTimer timer = new DispatcherTimer();

        List<IClickable> clickableObjects = new List<IClickable>();
        List<IClickable> selectedObjects = new List<IClickable>();

        DropOutStack<Action> undoStack = new DropOutStack<Action>(UndoBufferSize);
        DropOutStack<Action> redoStack = new DropOutStack<Action>(UndoBufferSize);

        List<Cable> cables = new List<Cable>();
        ContextGate contextGate;
        IClickable lastClickedObject;

        Pen backgroundGridPen;
        #endregion

        #region Gates
        public void CreateGate(Gate gate, int amtInputs, int amtOutputs)
        {
            contextGate.Context.Add(gate);
            gate.Renderer = new GateRenderer(canvas, gate);

            for (int i = 0; i < amtInputs; i++)
            {
                var inputNode = new InputNode(gate);
                gate.Input.Add(inputNode);
                clickableObjects.Add(inputNode);
                inputNode.Renderer = new ConnectionNodeRenderer(canvas, inputNode, gate, false);
            }
            for (int i = 0; i < amtOutputs; i++)
            {
                var outputNode = new OutputNode(gate);
                gate.Output.Add(outputNode);
                clickableObjects.Add(outputNode);
                outputNode.Renderer = new ConnectionNodeRenderer(canvas, outputNode, gate, true);
            }

            gate.Position = new Point(Math.Round(lastCanvasClick.X), Math.Round(lastCanvasClick.Y));

            //PerformAction(new CreateGateAction(contextGate, gate));

            Select(gate);
            clickableObjects.Add(gate);
            contextGate.Context.Add(gate);

            gate.UpdateConnectionNodePos();

            gate.Renderer.OnPositionChanged();
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
            {
                ticked.Tick(tickedNodes);
                ticked.Renderer.OnStateChanged();
            }
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
                selectedObjects.Add(obj);
                obj.IsSelected = true;
            }
        }
        public void Deselect(IClickable obj)
        {
            if (obj.IsSelected)
            {
                selectedObjects.Remove(obj);
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
            foreach (IClickable obj in selectedObjects)
                obj.IsSelected = false;
            selectedObjects.Clear();
        }

        public void Delete()
        {
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is Gate)
                    Remove(obj as Gate);
                else if (obj is ConnectionNode)
                    (obj as ConnectionNode).Clear();
            }
            DeselectAll();
        }
        public void Remove(Gate gate)
        {
            foreach (InputNode input in gate.Input)
                Remove(input);
            foreach (OutputNode output in gate.Output)
                Remove(output);
            clickableObjects.Remove(gate);
            contextGate.Context.Remove(gate);
        }
        public void Remove(ConnectionNode connectionNode)
        {
            connectionNode.Clear();
            clickableObjects.Remove(connectionNode);
        }

        public IClickable FindNearestObjectAt(Point pos)
        {
            IClickable nearest = null;
            double best = Double.PositiveInfinity;

            foreach (IClickable obj in clickableObjects.Where(obj => obj.Hitbox.IncludesPos(pos)))
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
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is InputSwitch)
                {
                    InputSwitch inputSwitch = obj as InputSwitch;
                    inputSwitch.State = !inputSwitch.State;
                    Tick(inputSwitch.Output[0]);
                    inputSwitch.Renderer.OnPositionChanged();
                }
                if (obj is ConnectionNode)
                {
                    ConnectionNode connectionNode = obj as ConnectionNode;
                    connectionNode.Invert();
                    Tick(connectionNode);
                }
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

        public void Rename()
        {

        }

        public void EmptyInput()
        {

        }
        public void InvertConnection()
        {
            foreach (IClickable obj in selectedObjects)
                if (obj is ConnectionNode)
                {
                    ConnectionNode connectionNode = obj as ConnectionNode;
                    connectionNode.Invert();
                    Tick(connectionNode);
                }
        }
        #endregion

        #region Visuals
        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(DefaultGridSize, DefaultGridSize);
            canvas.RenderTransform = new MatrixTransform(matrix);
            currentScale = 1;
        }
        public void UpdateTitle()
        {
            Title = $"{fileName}{(saved ? "" : " \u2022")} - {WindowTitle}";
        }

        public void DrawGrid()
        {
            double linewidth = LineWidth / currentScale;
            backgroundGridPen = new Pen(Brushes.LightGray, linewidth / 2);
            var geometry = new RectangleGeometry(new Rect(0, 0, 1, 1));
            var drawing = new GeometryDrawing(Brushes.White, backgroundGridPen, geometry);
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
            backgroundGridPen.Thickness = LineWidth / currentScale / 2;
            backgoundLayerCanvas.Background.Transform = canvas.RenderTransform;
        }

        public void Zoom(bool zoomIn, Point at)
        {
            double scale = zoomIn ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, at.X, at.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);

            currentScale *= scale;
            UpdateGrid();
        }
        public void ZoomIn()
        {
            Zoom(true, CanvasCenter);
        }
        public void ZoomOut()
        {
            Zoom(false, CanvasCenter);
        }
        #endregion

        #region Misc
        public void PerformAction(Action action)
        {
            saved = false;
            UpdateTitle();
            action.Redo();
            undoStack.Push(action);
            redoStack.Clear();
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
        public void UpdateClickableObjects()
        {
            clickableObjects.Clear();
            foreach (Gate gate in contextGate.Context)
            {
                clickableObjects.Add(gate);
                foreach (InputNode input in gate.Input)
                    clickableObjects.Add(input);
                foreach (OutputNode output in gate.Output)
                    clickableObjects.Add(output);
            }
        }

        public void Print()
        {

        }
        public void New()
        {
            contextGate = null;
        }
        public void Open()
        {
            New();
            var dialog = new OpenFileDialog();
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                contextGate = (ContextGate)StorageConverter.ToGate(Storage.Load(dialog.FileName));
                /*UpdateClickableObjects();
                foreach (Gate gate in contextGate.Context)
                {
                    gate.Renderer = new GateRenderer(canvas, gate);
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
                            cable.OutputNode = node;
                            cable.InputNode = inNode;
                            //cable.Renderer = new CableRenderer(canvas, cable);
                            addPoint(p1, true);
                            addPoint(p2, true);
                            cables.Add(cable);
                        }
                    }
                }*/
            }
        }

        public void Undo()
        {
            if (AllowUndo)
            {
                Action lastAction = undoStack.Pop();
                lastAction.Undo();
                redoStack.Push(lastAction);
            }
        }
        public void Redo()
        {
            if (AllowRedo)
            {
                Action lastAction = redoStack.Pop();
                lastAction.Redo();
                undoStack.Push(lastAction);
            }
        }

        public void Save()
        {
            if (currentFilePath != null && !saved)
                Storage.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
            else
                SaveAs();
            saved = true;
            UpdateTitle();
        }
        public void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = fileName;
            dialog.DefaultExt = FileExtention;
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                fileName = System.IO.Path.GetFileNameWithoutExtension(dialog.SafeFileName);
                Storage.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
                saved = true;
                UpdateTitle();
            }
        }
        #endregion

        #region Util
        public bool AllowUndo
        {
            get
            {
                return undoStack.Count > 0;
            }
        }
        public bool AllowRedo
        {
            get
            {
                return redoStack.Count > 0;
            }
        }

        public bool AnySelected
        {
            get
            {
                return selectedObjects.Count > 0;
            }
        }
        public bool AnyGateSelected
        {
            get
            {
                return selectedObjects.Exists(obj => obj is Gate);
            }
        }
        public bool AnyConnectionSelected
        {
            get
            {
                return selectedObjects.Exists(obj => obj is ConnectionNode);
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
                return Clipboard.ContainsData(FileExtention);
            }
        }

        public Point CanvasCenter
        {
            get
            {
                return TranslatePoint(new Point(Width / 2, Height / 2), canvas);
            }
        }
        #endregion

        #region Events
        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();

            lastCanvasClick = e.GetPosition(canvas);
            lastWindowClick = e.GetPosition(this);

            mouseMoved = false;

            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                movingScreen = true;
            }
            else
            {
                lastClickedObject = FindNearestObjectAt(lastCanvasClick);
                if (lastClickedObject == null)
                {
                    makingSelection = true;
                }
                else if (lastClickedObject is IMovable)
                {
                    movingObjects = true;
                }
                else
                {
                    makingSelection = true;
                }
            }

            if (makingSelection)
            {
                Canvas.SetLeft(selectVisual, lastWindowClick.X);
                Canvas.SetTop(selectVisual, lastWindowClick.Y);
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

            Vector windowMoved = currentWindowPos - lastWindowPos;
            Vector canvasMoved = e.GetPosition(canvas) - lastCanvasPos;

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
                    if (!lastClickedObject.IsSelected)
                    {
                        DeselectAll();
                        Select(lastClickedObject);
                    }
                    if (selectedObjects.Count == 0)
                    {
                        Select(lastClickedObject);
                    }
                    foreach (IClickable obj in selectedObjects)
                    {
                        if (obj is IMovable)
                            (obj as IMovable).Move(canvasMoved);
                    }
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

                    if (currentWindowPos.X < lastWindowClick.X)
                    {
                        Canvas.SetLeft(selectVisual, currentWindowPos.X);
                        selectVisual.Width = lastWindowClick.X - currentWindowPos.X;
                    }
                    else
                    {
                        selectVisual.Width = currentWindowPos.X - lastWindowClick.X;
                    }
                    if (currentWindowPos.Y < lastWindowClick.Y)
                    {
                        Canvas.SetTop(selectVisual, currentWindowPos.Y);
                        selectVisual.Height = lastWindowClick.Y - currentWindowPos.Y;
                    }
                    else
                    {
                        selectVisual.Height = currentWindowPos.Y - lastWindowClick.Y;
                    }
                }

                lastWindowPos = currentWindowPos;
                lastCanvasPos = e.GetPosition(canvas);
            }
            else if ((lastMousePos - lastWindowClick).LengthSquared >= MinPxMouseMoved * MinPxMouseMoved)
            {
                if (makingSelection && ControlPressed == false)
                {
                    DeselectAll();
                    SelectAllIn(new Rect(lastCanvasClick, lastCanvasPos));
                }
                mouseMoved = true;
            }

            lastMousePos = currentWindowPos;

            UpdateGrid();
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            if (mouseMoved)
            {
                if (makingSelection)
                {
                    selectVisual.Width = selectVisual.Height = 0;
                    selectVisual.Visibility = Visibility.Collapsed;
                }

                if (movingObjects)
                {
                    Vector completeMove = lastCanvasPos - lastCanvasClick;
                    foreach (IClickable obj in selectedObjects)
                    {
                        if (obj is IMovable)
                            (obj as IMovable).Move(-completeMove);
                    }

                    if (completeMove.LengthSquared > 0.5 * 0.5 * Unit * Unit)
                    {
                        var moveObjects = new CompoundAction();
                        foreach (IClickable obj in selectedObjects)
                        {
                            if (obj is IMovable)
                            {
                                moveObjects.Actions.Add(new MoveAction(obj as IMovable,
                                    new Vector(Math.Round(completeMove.X), Math.Round(completeMove.Y))));
                            }
                        }
                        if (moveObjects.Actions.Count > 0)
                        {
                            PerformAction(moveObjects);
                        }
                    }
                }
            }
            else
            {
                if (lastClickedObject == null)
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
                        bool islastClickedSelected = lastClickedObject.IsSelected;
                        if (islastClickedSelected == false)
                            Select(lastClickedObject);
                    }
                    else
                    {
                        if (ControlPressed == false && lastClickedObject.IsSelected == false)
                        {
                            DeselectAll();
                        }
                        Select(lastClickedObject);
                    }
                }
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
            creatingCable = false;
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom(e.Delta > 0, e.GetPosition(canvas));
        }

        void Window_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = mouseMoved;
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        void CreateInputSwitch(object sender, RoutedEventArgs e)
        {
            CreateGate(new InputSwitch(), 0, 1);
        }
        void CreateOutputLight(object sender, RoutedEventArgs e)
        {
            CreateGate(new OutputLight(), 1, 0);
        }
        void CreateAndGate(object sender, RoutedEventArgs e)
        {
            CreateGate(new AndGate(), 2, 1);
        }
        void CreateOrGate(object sender, RoutedEventArgs e)
        {
            CreateGate(new OrGate(), 2, 1);
        }
        void CreateNotGate(object sender, RoutedEventArgs e)
        {
            var newGate = new NopGate();
            CreateGate(newGate, 1, 1);
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
        }

        void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }
        void Open_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }
        void Print_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }
        void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        void Undo_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }
        void Copy_Click(object sender, RoutedEventArgs e)
        {
            Copy();
        }
        void Cut_Click(object sender, RoutedEventArgs e)
        {
            Cut();
        }
        void Paste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }
        void Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        void DefaultView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }
        void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }
        void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        void AddComp_Click(object sender, RoutedEventArgs e)
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
        void TrimInput_Click(object sender, RoutedEventArgs e)
        {
        }
        void InvertConnection_Click(object sender, RoutedEventArgs e)
        {
            InvertConnection();
        }
        void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }
        void ContextGate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = FileFilter
            };

            if (dialog.ShowDialog() == true)
            {
                Gate gate = StorageConverter.ToGate(Storage.Load(dialog.FileName));
                gate.Position = new Point(Math.Round(lastCanvasClick.X), Math.Round(lastCanvasClick.Y));
                gate.Renderer = new GateRenderer(canvas, gate);
                clickableObjects.Add(gate);
                contextGate.Context.Add(gate);
                Select(gate);
            }
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleObjects();
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
        void Rerender_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in contextGate.Context)
            {
                gate.Renderer.Unrender();
                gate.Renderer.Render();
                gate.Renderer.OnPositionChanged();
            }
        }
        #endregion
    }
}
