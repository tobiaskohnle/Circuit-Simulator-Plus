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
        Point lastMouseClick;
        bool dragging = false;

        //double scale = 1.0;
        //Point position;

        /// <summary>
        /// ConnectionNodes scheduled to be updated.
        /// </summary>
        Queue<ConnectionNode> tickedNodes = new Queue<ConnectionNode>();
        List<Gate> selected = new List<Gate>();
        //SimulationContext context;
        DispatcherTimer timer;
        //List<Gate> gates = new List<Gate>();
        Gate mainGate = new Gate();
        List<Cable> cables = new List<Cable>();
        bool drawingcable;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //canvas.SnapsToDevicePixels = true;

            ResetView();

            Title = WindowTitle;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                mainGate = Storage.Load(args[1]);
            else
                mainGate = new Gate();
            foreach (Gate gate in mainGate.Context)
                gate.Renderer.Render();

            //testing <--
            //var grid = new Grid(canvas, (int)Width, (int)Height);
            //grid.Draw();
            // -->

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (sender, e) =>
            {
                DEBUG_TickAll();
            };
            timer.Start();

            DEBUG_Test1();
            //DEBUG_Test2();
            //DEBUG_Test3();
            //DEBUG_Test4();
            //MessageBox.Show("All Tests completed.");
        }

        public Gate DEBUG_CreateGate(Gate gate, int amtInputs, int amtOutputs)
        {
            mainGate.Context.Add(gate);
            var renderer = new SimpleGateRenderer(canvas, gate);
            renderer.InputClicked += OnGateInputClicked;
            renderer.OutputClicked += OnGateOutputClicked;
            gate.Renderer = renderer;
            gate.Position = new Point(5, mainGate.Context.Count * 5);

            for (int i = 0; i < amtInputs; i++)
                gate.Input.Add(new InputNode(gate));
            for (int i = 0; i < amtOutputs; i++)
                gate.Output.Add(new OutputNode(gate));

            gate.Renderer.Render();
            return gate;
        }

        public void DEBUG_Test1()
        {
            mainGate.Context.Clear();
            DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);
            DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);

            mainGate.Context[0].Input[0].State = true;
            mainGate.Context[0].Output[0].ConnectTo(mainGate.Context[1].Input[0]);
            mainGate.Context[0].Input[0].Tick(tickedNodes);

            //DEBUG_TickAll();

            DEBUG_CheckStates(mainGate.Context.ToArray(), new[] { true, false, true, true, false, true });
        }

        public void DEBUG_Test2()
        {
            mainGate.Context.Clear();
            DEBUG_CreateGate(new Gate(Gate.GateType.And), 2, 1);

            mainGate.Context[0].Output[0].Invert();
            mainGate.Context[0].Output[0].Tick(tickedNodes);

            //DEBUG_TickAll();

            DEBUG_CheckStates(mainGate.Context.ToArray(), new[] { false, false, true });
        }

        public void DEBUG_Test3()
        {
            mainGate.Context.Clear();
            Gate a = DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);
            Gate b = DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);

            a.Output[0].Invert();
            b.Output[0].Invert();

            a.Output[0].ConnectTo(b.Input[0]);
            b.Output[0].ConnectTo(a.Input[1]);

            a.Output[0].Tick(tickedNodes);
            DEBUG_TickAll();
            b.Output[0].Tick(tickedNodes);
            DEBUG_TickAll();

            DEBUG_CheckStates(mainGate.Context.ToArray(), new[] { false, false, true, true, false, false });

            a.Input[0].State = true;
            a.Input[0].Tick(tickedNodes);
            //DEBUG_TickAll();

            DEBUG_CheckStates(mainGate.Context.ToArray(), new[] { true, true, false, false, false, true });

            a.Input[0].State = false;
            a.Input[0].Tick(tickedNodes);
            //DEBUG_TickAll();

            DEBUG_CheckStates(mainGate.Context.ToArray(), new[] { false, true, false, false, false, true });
        }

        public void DEBUG_Test4()
        {
            Gate xor = DEBUG_CreateGate(new Gate(), 2, 1);
            Gate and0 = DEBUG_CreateGate(new Gate(Gate.GateType.And), 2, 1);
            Gate and1 = DEBUG_CreateGate(new Gate(Gate.GateType.And), 2, 1);
            Gate or = DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);

            xor.Position = new Point(0, 5);
            and0.Position = new Point(5, 2);
            and1.Position = new Point(5, 8);
            or.Position = new Point(10, 5);

            xor.Context.Add(and0);
            xor.Context.Add(and1);
            xor.Context.Add(or);

            and0.Input[0].Invert();
            and1.Input[0].Invert();
            and0.Output[0].ConnectTo(or.Input[0]);
            and1.Output[0].ConnectTo(or.Input[1]);

            xor.Input[0].ConnectTo(and0.Input[0]);
            xor.Input[0].ConnectTo(and1.Input[1]);
            xor.Input[1].ConnectTo(and0.Input[1]);
            xor.Input[1].ConnectTo(and1.Input[0]);
            or.Output[0].ConnectTo(xor.Output[0]);

            mainGate.Context.Clear();
            mainGate.Context.Add(xor);
        }

        public void DEBUG_CheckStates(Gate[] gates, bool[] states)
        {
            return;
            int state_i = 0;
            foreach (Gate gate in gates)
            {
                foreach (InputNode input in gate.Input)
                    if (input.State != states[state_i++])
                        throw new InvalidOperationException();
                foreach (OutputNode output in gate.Output)
                    if (output.State != states[state_i++])
                        throw new InvalidOperationException();
            }
            if (state_i != states.Length)
                throw new InvalidOperationException();
        }

        public void DEBUG_TickAll()
        {
            //while (tickedNodes.Any())
            //{
            List<ConnectionNode> copy = tickedNodes.ToList();
            tickedNodes.Clear();
            foreach (ConnectionNode ticked in copy)
                ticked.Tick(tickedNodes);
            //}
        }

        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(20, 20);
            canvas.RenderTransform = new MatrixTransform(matrix);
        }

        public void PerformAction(Action action)
        {
            // TODO: Create undo / redo stack
            action.Redo();
        }

        #region Events
        void DEBUG_AddAndGate(object sender, EventArgs e)
        {
            DEBUG_CreateGate(new Gate(Gate.GateType.And), 2, 1);//.Position = lastMouseClick;
        }
        void DEBUG_AddOrGate(object sender, EventArgs e)
        {
            DEBUG_CreateGate(new Gate(Gate.GateType.Or), 2, 1);//.Position = lastMouseClick;
        }
        void DEBUG_AddNotGate(object sender, EventArgs e)
        {
            DEBUG_CreateGate(new Gate(Gate.GateType.Not), 2, 1);//.Position = lastMouseClick;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                mainGate.Context[0].Input[0].State = !mainGate.Context[0].Input[0].State;
                mainGate.Context[0].Input[0].Tick(tickedNodes);
            }
            if (e.Key == Key.B)
            {
                mainGate.Context[0].Input[1].State = !mainGate.Context[0].Input[1].State;
                mainGate.Context[0].Input[1].Tick(tickedNodes);
            }
            //if (e.Key == Key.C)
            //{
            //    mainGate.Context[1].Input[1].State = true;
            //    mainGate.Context[1].Input[1].Tick(tickedNodes);
            //}
            //if (e.Key == Key.D)
            //{
            //    mainGate.Context[1].Input[1].State = false;
            //    mainGate.Context[1].Input[1].Tick(tickedNodes);
            //}
        }
        void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePos = lastMouseClick = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragging = true;
                CaptureMouse();
            }
            if (drawingcable)
            {
                Cable lastcable = cables.Last();
                lastcable.AddPoint(e.GetPosition(canvas));
            }
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(this);
            Vector moved = currentPos - lastMousePos;

            if (dragging)
            {
                Matrix matrix = canvas.RenderTransform.Value;
                matrix.Translate(moved.X, moved.Y);
                canvas.RenderTransform = new MatrixTransform(matrix);
            }

            lastMousePos = currentPos;
            //position = Mouse.GetPosition(canvas);
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            ReleaseMouseCapture();
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
            double scale = e.Delta > 0 ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, currentPos.X, currentPos.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);
            //scale = e.Delta > 0 ? scale + 2 : scale-2;

            //scale = e.Delta > 1 ? 0 : scale + 1;
            //ScaleTransform scle = new ScaleTransform(scale, scale, 1/position.X, 1/position.Y);
            //canvas.RenderTransform = scle;
            //e.Handled = true;
        }

        void NewFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in mainGate.Context)
            {
                gate.Renderer.Unrender();
            }
        }
        void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            NewFile_Click(sender, e);
            var dialog = new OpenFileDialog();
            dialog.Filter = "Circuit File (.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                mainGate = Storage.Load(dialog.FileName);
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
                Storage.Save(path, mainGate);
            }
        }
        void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {

        }

        void Undo_Click(object sender, RoutedEventArgs e)
        {

        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {

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

        void OnGateOutputClicked(object sender, EventArgs e)
        {
            Gate gate = (Gate)sender;
            int index = ((IndexEventArgs)e).Index;

            Point point = new Point();
            point.X = gate.Position.X + 3 + 1;
            point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Output.Count);
            Cable cable = new Cable();
            cable.AddPoint(point);
            cables.Add(cable);
            cable.Renderer = new CableRenderer(canvas, cable);
            cable.Output = gate.Output[index];
            drawingcable = true;
        }

        void OnGateInputClicked(object sender, EventArgs e)
        {
            Gate gate = (Gate)sender;
            int index = ((IndexEventArgs)e).Index;

            Point point = new Point();
            point.X = gate.Position.X - 1;
            point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Input.Count);
            Cable lastcable = cables.Last();
            lastcable.AddPoint(point);
            lastcable.Input = gate.Input[index];
            drawingcable = false;
        }
        #endregion
    }
}
