using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace CircuitSimulatorPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Canvas = canvas;

            DrawGrid();
            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                contextGate = (ContextGate)StorageConverter.ToGate(StorageUtil.Load(args[1]));
            else
                contextGate = new ContextGate();

            UpdateClickableObjects();

            timer.Tick += TimerTick;

            canvas.Children.Add(newCable);
            Panel.SetZIndex(newCable, 1);
        }

        #region Constants
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileExtention;
        public const string DefaultTitle = "untitled";
        public const string FileExtention = ".tici";
        public const double MinPxMouseMoved = 5;
        public const double DefaultGridSize = 20;
        public const double ScaleFactor = 0.9;
        public const double LineRadius = 1d / 20;
        public const double LineWidth = 1d / 10;
        public const double InversionDotRadius = 1d / 4;
        public const double InversionDotDiameter = 1d / 2;
        public const double CableJointSize = 1d / 3;
        public const double ConnectionNodeLineLength = 1d;
        public const int UndoBufferSize = 32;
        #endregion

        #region Properties
        public static Canvas Canvas;

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

        Line newCable = new Line
        {
            Stroke = Brushes.DarkTurquoise,
            StrokeThickness = LineWidth,
            StrokeDashArray = new DoubleCollection { DefaultGridSize / 2, DefaultGridSize / 2 }
        };

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

        #region Object
        public void CreateGate(Gate gate, int amtInputs, int amtOutputs)
        {
            for (int i = 0; i < amtInputs; i++)
            {
                var inputNode = new InputNode(gate);
                //inputNode.Renderer = new ConnectionNodeRenderer(canvas, inputNode, gate, false);
                gate.Input.Add(inputNode);
                clickableObjects.Add(inputNode);
            }
            for (int i = 0; i < amtOutputs; i++)
            {
                var outputNode = new OutputNode(gate);
                //outputNode.Renderer = new ConnectionNodeRenderer(canvas, outputNode, gate, true);
                gate.Output.Add(outputNode);
                clickableObjects.Add(outputNode);
            }

            gate.Position = new Point(Math.Round(lastCanvasClick.X), Math.Round(lastCanvasClick.Y));

            gate.UpdateConnectionNodePos();

            //PerformAction(new CreateGateAction(contextGate, gate));

            Select(gate);
            clickableObjects.Add(gate);
            contextGate.Context.Add(gate);
        }

        public void Tick(ConnectionNode node)
        {
            tickedNodes.Enqueue(node);
            if (singleTicks == false)
                timer.Start();
        }
        public void TickQueue()
        {
            Console.WriteLine("Tick");
            List<ConnectionNode> tickedNodesCopy = tickedNodes.ToList();
            tickedNodes.Clear();
            foreach (ConnectionNode ticked in tickedNodesCopy)
            {
                ticked.Tick(tickedNodes);
                //ticked.Renderer.OnStateChanged();
            }
            if (tickedNodes.Count == 0)
                timer.Stop();
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
                {
                    Remove(obj as Gate);
                }
                else if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Clear();
                }
                else if (obj is CableJoint)
                {
                    Remove(obj as CableJoint);
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
            clickableObjects.Remove(gate);
            contextGate.Context.Remove(gate);
        }
        public void Remove(CableJoint cableJoint)
        {
            var cableSegment = cableJoint.Before;
            cableSegment.B = cableJoint.After.B;
            cableSegment.B.Before = cableSegment;
            clickableObjects.Remove(cableJoint.After);
            clickableObjects.Remove(cableJoint);
        }
        public void Remove(ConnectionNode connectionNode)
        {
            connectionNode.IsRendered = false;
            connectionNode.Clear();
            clickableObjects.Remove(connectionNode);
        }

        public void Flip()
        {
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is Gate)
                {
                    var gate = obj as Gate;
                    gate.Size = new Size(gate.Size.Height, gate.Size.Width);

                    var u = gate.ConnectedNodes[ConnectionNode.Align.U].ToList();
                    var l = gate.ConnectedNodes[ConnectionNode.Align.L].ToList();
                    var d = gate.ConnectedNodes[ConnectionNode.Align.D].ToList();
                    var r = gate.ConnectedNodes[ConnectionNode.Align.R].ToList();

                    foreach (var node in u)
                        node.Alignment = ConnectionNode.Align.L;
                    foreach (var node in l)
                        node.Alignment = ConnectionNode.Align.U;
                    foreach (var node in d)
                        node.Alignment = ConnectionNode.Align.R;
                    foreach (var node in r)
                        node.Alignment = ConnectionNode.Align.D;

                    gate.UpdateConnectionNodePos();
                }
            }
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

        void SplitCables()
        {
            foreach (IClickable obj in selectedObjects.ToList())
            {
                if (obj is CableSegment)
                {
                    var cableSegment = obj as CableSegment;
                    var newSegment = new CableSegment();
                    var centerJoint = new CableJoint();

                    Point center = cableSegment.A.Position + (cableSegment.B.Position - cableSegment.A.Position) / 2;
                    center.X = Math.Round(center.X);
                    center.Y = Math.Round(center.Y);
                    centerJoint.Position = center;

                    newSegment.B = cableSegment.B;
                    cableSegment.B = centerJoint;
                    newSegment.A = cableSegment.B = centerJoint;

                    newSegment.B.Before = newSegment;
                    centerJoint.Before = cableSegment;
                    centerJoint.After = newSegment;

                    Select(newSegment);

                    //newSegment.Renderer = new CableSegmentRenderer(canvas, newSegment);
                    //centerJoint.Renderer = new CableJointRenderer(canvas, centerJoint);

                    clickableObjects.Add(centerJoint);
                    clickableObjects.Add(newSegment);
                }
            }
        }
        void ConnectAllNodes()
        {
            foreach (OutputNode outputNode in selectedObjects.Where(obj => obj is OutputNode))
                foreach (InputNode inputNode in selectedObjects.Where(obj => obj is InputNode))
                    outputNode.ConnectTo(inputNode);
        }
        void ConnectOppositeNodes()
        {
            var outputs = selectedObjects.Where(obj => obj is OutputNode).ToList();
            var inputs = selectedObjects.Where(obj => obj is InputNode).ToList();

            for (int i = 0; i < Math.Min(inputs.Count, outputs.Count); i++)
                (outputs[i] as OutputNode).ConnectTo(inputs[i] as InputNode);
        }

        public void Align(ConnectionNode.Align alignment)
        {
            foreach (IClickable obj in selectedObjects)
                if (obj is ConnectionNode)
                    (obj as ConnectionNode).Alignment = alignment;
        }
        #endregion

        #region IO
        public void New()
        {
            contextGate = null;
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
            if (currentFilePath != null)
                StorageUtil.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
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
                StorageUtil.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
                saved = true;
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
            matrix.Scale(DefaultGridSize, DefaultGridSize);
            canvas.RenderTransform = new MatrixTransform(matrix);
            currentScale = 1;
            UpdateGrid();
        }
        public void UpdateTitle()
        {
            Title = $"{fileName}{(saved ? "" : " \u2022")} - {WindowTitle}";
        }

        public void DrawGrid()
        {
            backgroundGridPen = new Pen(Brushes.LightGray, 0);
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

        public void ZoomIntoView(List<IClickable> objects)
        {
            if (objects.Count > 0)
            {
                double minX = Double.PositiveInfinity;
                double minY = Double.PositiveInfinity;
                double maxX = -Double.PositiveInfinity;
                double maxY = -Double.PositiveInfinity;

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

                matrix.ScaleAtPrepend(scale, scale, CanvasCenter.X, CanvasCenter.Y);
                matrix.TranslatePrepend(CanvasCenter.X - centerX, CanvasCenter.Y - centerY);
                canvas.RenderTransform = new MatrixTransform(matrix);

                currentScale *= scale;
                UpdateGrid();
            }
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

        public void MoveObjects()
        {
            Vector completeMove = lastCanvasPos - lastCanvasClick;
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is IMovable)
                    (obj as IMovable).Move(-completeMove);
            }

            var movedObjects = new List<IMovable>();

            foreach (IClickable obj in selectedObjects)
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
                    PerformAction(new MoveAction(movedObjects, completeMove));
                }
            }
        }
        public void CreateCable()
        {
            newCable.Visibility = Visibility.Collapsed;
            var startNode = lastClickedObject as ConnectionNode;

            IClickable clickedObject = FindNearestObjectAt(lastCanvasPos);

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

                    var cable = new Cable(endNode as InputNode, startNode as OutputNode);

                    Console.WriteLine($"Created cable");
                    startNode.ConnectTo(endNode);
                    Tick(endNode);
                }
            }
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
                Matrix matrix = canvas.RenderTransform.Value;
                matrix.Invert();
                return matrix.Transform(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2));
            }
        }
        #endregion

        #region Event Handlers
        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();

            lastCanvasClick = e.GetPosition(canvas);
            lastWindowClick = e.GetPosition(this);

            mouseMoved = false;
            lastClickedObject = FindNearestObjectAt(lastCanvasClick);

            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                movingScreen = true;
            }
            else
            {
                if (lastClickedObject == null)
                {
                    makingSelection = true;
                }
                else if (lastClickedObject is IMovable)
                {
                    movingObjects = true;
                }
                else if (lastClickedObject is ConnectionNode)
                {
                    newCable.Visibility = Visibility.Visible;
                    newCable.X1 = newCable.X2 = (lastClickedObject as ConnectionNode).Position.X;
                    newCable.Y1 = newCable.Y2 = (lastClickedObject as ConnectionNode).Position.Y;
                    creatingCable = true;
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

                if (creatingCable)
                {
                    newCable.X2 = lastCanvasPos.X;
                    newCable.Y2 = lastCanvasPos.Y;

                    newCable.StrokeDashOffset = DefaultGridSize * -0.25 * Math.Sqrt(
                        Math.Pow(newCable.X1 - newCable.X2, 2) + Math.Pow(newCable.Y1 - newCable.Y2, 2)
                    );
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
                    MoveObjects();
                }

                if (creatingCable)
                {
                    CreateCable();
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
                        {
                            Select(lastClickedObject);
                        }
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

            makingSelection = false;
            movingObjects = false;
            movingScreen = false;
            creatingCable = false;
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
            e.Handled = mouseMoved;
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        void TimerTick(object sender, EventArgs e)
        {
            TickQueue();
        }
        #endregion

        #region UI Event Handlers
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
            New();
            var dialog = new OpenFileDialog();
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                contextGate = (ContextGate)StorageConverter.ToGate(StorageUtil.Load(dialog.FileName));
                UpdateClickableObjects();
            }
        }
        void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilePath != null)
            {
                Process.Start(System.IO.Path.GetDirectoryName(currentFilePath));
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
            if (AnySelected)
            {
                DeselectAll();
            }
            else
            {
                SelectAll();
            }
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
            ZoomIntoView(selectedObjects);
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
            Console.WriteLine("ResizeGate...");
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

        }
        void TrimInput_Click(object sender, RoutedEventArgs e)
        {
        }
        void Align_Click(object sender, RoutedEventArgs e)
        {
            Align(ConnectionNode.Align.U);
        }

        void Version_Click(object sender, RoutedEventArgs e)
        {

        }
        void GithubLink_Click(object sender, RoutedEventArgs e)
        {

        }

        void Reload_Click(object sender, RoutedEventArgs e)
        {
        }
        void SingleTicks_Checked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Single Ticks Enabled");
        }
        void SingleTicks_Unchecked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Single Ticks Disabled");
            if (tickedNodes.Count > 0)
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
        #endregion
    }
}
