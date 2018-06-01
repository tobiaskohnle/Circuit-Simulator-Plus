using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            drawgrid(ScaleFactor);
        }
        double linewidth = LineWidth;

        public void drawgrid(double scale)
        {
           
            linewidth = linewidth / scale;
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
        #region Constants
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileFormat;
        public const string FileFormat = "tici";
        public const string DefaultTitle = "untitled";
        public const string Unsaved = "\u2022";
        public const double ScaleFactor = 0.9;
        public const double LineWidth = 0.1;
        public const int UndoBufferSize = 32;
        #endregion

        #region Properties
        Point lastMousePos;
        Point lastCanvasPos;
        Point lastMouseClick;
        Point lastCanvasClick;
        bool showContextMenu;
        bool drawingcable;
        bool dragging;
        bool saved = true;
        string title = DefaultTitle;

        Queue<ConnectionNode> tickedNodes = new Queue<ConnectionNode>();
        DispatcherTimer timer = new DispatcherTimer();
        List<Cable> cables = new List<Cable>();
        List<IClickable> selected = new List<IClickable>();
        List<Action> Undo = new List<Action>();
        List<Action> Redo = new List<Action>();
        Gate contextGate = new Gate();
        List<IClickable> clickableObjects = new List<IClickable>();
        #endregion

        #region Gates
        public void Add(Gate gate)
        {
            contextGate.Context.Add(gate);
            clickableObjects.Add(gate);
        }
        public void Add(ConnectionNode connectionNode)
        {
            clickableObjects.Add(connectionNode);
        }
        public Gate CreateGate(Gate gate, int amtInputs, int amtOutputs)
        {
            contextGate.Context.Add(gate);
            gate.Renderer = new GateRenderer(canvas, gate, OnGateInputClicked, OnGateOutputClicked);
            gate.Position = new Point(5, contextGate.Context.Count * 5);

            for (int i = 0; i < amtInputs; i++)
                gate.Input.Add(new InputNode(gate));
            for (int i = 0; i < amtOutputs; i++)
                gate.Output.Add(new OutputNode(gate));

            gate.Renderer.Render();

            var CreateGateAction = new CreateGateAction(gate, gate.Type, gate.Position, contextGate.Context, "Create Gate");
            Undo.Add(CreateGateAction);

            Add(gate);

            return gate;
        }

        public void Tick(ConnectionNode node)
        {
            tickedNodes.Enqueue(node);
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
        public void SelectAllIn(Rect rect)
        {
            foreach (IClickable obj in clickableObjects)
                if (obj.Hitbox.IsIncludedIn(rect))
                    Select(obj);
        }
        public void UnselectAll()
        {
            foreach (IClickable obj in selected)
                obj.IsSelected = false;
            selected.Clear();
        }
        public void SnapSelectedToGrid()
        {
            foreach (Gate gate in selected)
                gate.SnapToGrid();
        }

        public void Delete(Gate gate)
        {
            foreach (InputNode input in gate.Input)
                input.Clear();
            foreach (OutputNode output in gate.Output)
                output.Clear();
            gate.Renderer.Unrender();
            contextGate.Context.Remove(gate);

            DeleteGateAction DeleteGateAction = new DeleteGateAction(gate,gate.Type,gate.Position, contextGate.Context, "Gate deleted");
            Undo.Add(DeleteGateAction);
        }

        public List<IClickable> FindObjectsAt(Point pos)
        {
            return clickableObjects.Where(obj => obj.Hitbox.IncludesPos(pos)).ToList();
        }
        public IClickable FindNearestObjectAt(Point pos)
        {
            IClickable nearest = null;
            double best = Double.PositiveInfinity;

            foreach (IClickable obj in FindObjectsAt(pos))
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
        #endregion

        #region Visuals
        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(20, 20);
            canvas.RenderTransform = new MatrixTransform(matrix);
        }
        public void UpdateTitle()
        {
            Title = $"{title}{(saved ? "" : " " + Unsaved)} - {WindowTitle}";
        }
        #endregion

        #region Misc
        public void PerformAction(Action action)
        {
            saved = false;
            UpdateTitle();
            action.Redo();
        }
        public void RevokeAction(Action action)
        {
            action.Undo();
        }

        #endregion

        #region Events
        void DEBUG_AddAndGate(object sender, EventArgs e)
        {
            CreateGate(new Gate(Gate.GateType.And), 2, 1).Position = lastCanvasClick;
            foreach (Gate gate in contextGate.Context)
                gate.SnapToGrid();
        }
        void DEBUG_AddNandGate(object sender, EventArgs e)
        {
            var newGate = CreateGate(new Gate(Gate.GateType.And), 2, 1);
            newGate.Position = lastCanvasClick;
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
            foreach (Gate gate in contextGate.Context)
                gate.SnapToGrid();
        }
        void DEBUG_AddOrGate(object sender, EventArgs e)
        {
            CreateGate(new Gate(Gate.GateType.Or), 2, 1).Position = lastCanvasClick;
            foreach (Gate gate in contextGate.Context)
                gate.SnapToGrid();
        }
        void DEBUG_AddNorGate(object sender, EventArgs e)
        {
            var newGate = CreateGate(new Gate(Gate.GateType.Or), 2, 1);
            newGate.Position = lastCanvasClick;
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
            foreach (Gate gate in contextGate.Context)
                gate.SnapToGrid();
        }
        void DEBUG_AddNotGate(object sender, EventArgs e)
        {
            Gate gate = CreateGate(new Gate(Gate.GateType.Identity), 1, 1);
            gate.Position = lastCanvasClick;
            gate.Output[0].Invert();
            gate.Output[0].Tick(tickedNodes);
            gate.SnapToGrid();
        }
        void DEBUG_AddIdentityGate(object sender, EventArgs e)
        {
            Gate gate = CreateGate(new Gate(Gate.GateType.Identity), 1, 1);
            gate.Position = lastCanvasClick;
            gate.SnapToGrid();
        }
        void DEBUG_DeleteGate(object sender, EventArgs e)
        {
            IClickable clickedObject = FindNearestObjectAt(lastCanvasClick);
            if (clickedObject != null && clickedObject is Gate)
            {
                Delete(clickedObject as Gate);
            }
        }
        void DEBUG_ToggleGate(object sender, EventArgs e)
        {
            IClickable clickedObject = FindNearestObjectAt(lastCanvasClick);
            if (clickedObject != null && clickedObject is Gate)
            {
                Gate clicked = clickedObject as Gate;
                clicked.Input[0].State = !clicked.Input[0].State;
                clicked.Output[0].State = !clicked.Output[0].State;
                Tick(clicked.Output[0]);
            }
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
        }
        void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastCanvasClick = e.GetPosition(canvas);

            lastMousePos = lastMouseClick = e.GetPosition(this);

            IClickable clickedObject = FindNearestObjectAt(lastCanvasClick);

            if (clickedObject is Gate)
            {
                Gate clicked = clickedObject as Gate;
                Select(clicked);
            }
            else if (clickedObject is ConnectionNode)
            {
                ConnectionNode clicked = clickedObject as ConnectionNode;
                clicked.Invert();
            }

            showContextMenu = true;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                dragging = true;
                CaptureMouse();
            }
            if (drawingcable)
            {
                Cable lastcable = cables.Last();
                Point pos = e.GetPosition(canvas);
                pos.X = Math.Round(pos.X);
                pos.Y = Math.Round(pos.Y);
                lastcable.AddPoint(pos);
            }
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(this);
            Vector moved = currentPos - lastMousePos;
            Vector canvasMoved = e.GetPosition(canvas) - lastCanvasPos;

            if (dragging)
            {
                Matrix matrix = canvas.RenderTransform.Value;
                matrix.Translate(moved.X, moved.Y);
                canvas.RenderTransform = new MatrixTransform(matrix);
            }

            if (e.LeftButton == MouseButtonState.Pressed)
                foreach (Gate gate in selected)
                    gate.Move(canvasMoved);

            lastMousePos = currentPos;
            lastCanvasPos = e.GetPosition(canvas);

            if (moved.Length > 0)
                showContextMenu = false;
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SnapSelectedToGrid();
            UnselectAll();
            dragging = false;
            foreach (Gate gate in selected)
                gate.SnapToGrid();
            ReleaseMouseCapture();
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
            double scale = e.Delta > 0 ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, currentPos.X, currentPos.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);
            drawgrid(scale);
        }

        void Window_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = !showContextMenu;
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
            if (Undo.Count() > 0)
            {
                RevokeAction(Undo.Last());
                Redo.Add(Undo.Last());
                Undo.Remove(Undo.Last());
            }
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (Redo.Count() > 0)
            {
                PerformAction(Redo.Last());
                Undo.Add(Redo.Last());
                Redo.Remove(Redo.Last());
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
        void RenameGate_Click(object sender, RoutedEventArgs e)
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

        void Reset_Click(object sender, RoutedEventArgs e)
        {

        }
        void Reload_Click(object sender, RoutedEventArgs e)
        {
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

            if (!drawingcable)
            {
                Point point = new Point();
                point.X = gate.Position.X + 3 + 1;
                point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Output.Count);
                Cable cable = new Cable();
                cables.Add(cable);
                cable.Renderer = new CableRenderer(canvas, cable);
                cable.Output = gate.Output[index];
                drawingcable = true;
            }
        }
        void OnGateInputClicked(object sender, EventArgs e)
        {
            Gate gate = (Gate)sender;
            int index = ((IndexEventArgs)e).Index;

            if (drawingcable)
            {
                Point point = new Point();
                point.X = gate.Position.X - 1;
                point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Input.Count);
                Cable lastcable = cables.Last();
                lastcable.AddPoint(point, true);
                lastcable.Input = gate.Input[index];
                lastcable.Output.ConnectTo(lastcable.Input);
                Tick(lastcable.Input);
                drawingcable = false;
            }
        }
        #endregion
    }
}
