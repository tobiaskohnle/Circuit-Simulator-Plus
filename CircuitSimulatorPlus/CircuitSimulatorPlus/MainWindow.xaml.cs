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

        List<Gate> selected;
        SimulationContext context;
        List<Gate> gates = new List<Gate>();
        #endregion

        public static int id = 0;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            canvas.SnapsToDevicePixels = true;

            ResetView();

            Title = WindowTitle;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                context = Storage.Load(args[1]);
            else
                context = new SimulationContext();

            var or_el_gate = new OrElementaryGate();
            var and_el_gate = new AndElementaryGate();

            var or_gate = new Gate();
            or_gate.Position = new Point(0, 0);
            or_gate.input.Add(new InputNode());
            or_gate.input.Add(new InputNode());
            or_gate.output.Add(new OutputNode());

            or_gate.input[0].repr = null;
            or_gate.input[1].repr = null;
            or_gate.input[0].state = or_el_gate;
            or_gate.input[1].state = or_el_gate;
            or_gate.output[0].repr = or_el_gate;
            or_gate.output[0].state = or_el_gate;

            or_gate.Renderer = new SimpleGateRenderer(canvas, or_gate);
            context.Add(or_gate);

            var and_gate = new Gate();

            and_gate.Position = new Point(5, 5);
            and_gate.input.Add(new InputNode());
            and_gate.input.Add(new InputNode());
            and_gate.output.Add(new OutputNode());

            and_gate.input[0].repr = or_el_gate;
            and_gate.input[1].repr = or_el_gate;
            and_gate.input[0].state = and_el_gate;
            and_gate.input[1].state = and_el_gate;
            and_gate.output[0].repr = and_el_gate;
            and_gate.output[0].state = and_el_gate;

            and_gate.Renderer = new SimpleGateRenderer(canvas, and_gate);
            context.Add(and_gate);

            or_gate.output[0].ConnectTo(and_gate.input[0]);
            or_gate.output[0].Clear();

            //var gate1 = new Gate();
            //gate1.Renderer = new SimpleGateRenderer(canvas, gate1);
            //gate1.Position = new Point(40, 0);
            //context.Add(gate1);
            //gate1.Position = new Point(40, 20);
            //context.Remove(gate0);
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
        void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }
        void Window_KeyUp(object sender, KeyEventArgs e)
        {

        }

        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePos = lastMouseClick = e.GetPosition(this);
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(this);
            Vector moved = currentPos - lastMousePos;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                Matrix matrix = canvas.RenderTransform.Value;
                matrix.Translate(moved.X, moved.Y);
                canvas.RenderTransform = new MatrixTransform(matrix);
            }

            lastMousePos = currentPos;
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
            double scale = e.Delta > 0 ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, currentPos.X, currentPos.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);
        }

        void NewFile_Click(object sender, RoutedEventArgs e)
        {

        }
        void OpenFile_Click(object sender, RoutedEventArgs e)
        {

        }
        void SaveFile_Click(object sender, RoutedEventArgs e)
        {

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
        #endregion
    }
}
