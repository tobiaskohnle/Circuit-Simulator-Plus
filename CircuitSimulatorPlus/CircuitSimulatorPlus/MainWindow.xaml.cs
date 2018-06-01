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
        Grid grid;
        public MainWindow()
        {
            InitializeComponent();
            //grid = new Grid(canvas, 992, 648, 1.0);
            //grid.Render();
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


        }
        double rescale;

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
        List<Gate> selected = new List<Gate>();
        List<Action> Undo = new List<Action>();
        List<Action> Redo = new List<Action>();
        Gate contextGate = new Gate();
        #endregion

        #region Gates
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
        public void Select(Gate gate)
        {
            if (!gate.IsSelected)
            {
                selected.Add(gate);
                gate.IsSelected = true;
            }
        }
        public void UnselectAll()
        {
            foreach (Gate gate in selected)
                gate.IsSelected = false;
            selected.Clear();
        }
        public void SnapSelectedToGrid()
        {
            foreach (Gate gate in selected)
                gate.SnapToGrid();
        }
        public Gate GateAt(Point pos)
        {
            foreach (Gate gate in contextGate.Context)
            {
                if (gate.Position.X <= pos.X
                    && gate.Position.Y <= pos.Y
                    && gate.Position.Y + gate.Size.Width >= pos.Y
                    && gate.Position.X + gate.Size.Height >= pos.X)
                {
                    return gate;
                }
            }
            return null;
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
        public void Gridlinewidth()
        {
            grid.Gridlinewidth();
        }
        #endregion

        #region Misc
        public void PerformAction(Action action)
        {
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
            //CreateGate(new Gate(Gate.GateType.Not), 1, 1).Position = lastCanvasClick;
            //foreach (Gate gate in contextGate.Context)
            //    gate.SnapToGrid();
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
            Gate clicked = GateAt(lastCanvasClick);
            if (clicked != null)
                Delete(clicked);
        }
        void DEBUG_ToggleGate(object sender, EventArgs e)
        {
            Gate clicked = GateAt(lastCanvasClick);
            if (clicked != null)
            {
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

            Gate clicked = GateAt(lastCanvasClick);
            if (clicked != null)
                Select(clicked);

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
                //grid.position(moved.X, moved.Y);

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

            //grid.scale = scale;
            //grid.Gridlinewidth();

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
            RevokeAction(Undo.Last());
            Redo.Add(Undo.Last());
            Undo.Remove(Undo.Last());
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            PerformAction(Undo.Last());
            Undo.Add(Redo.Last());
            Redo.Remove(Redo.Last());
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
