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
        const string WindowTitle = "Circuit Simulator Plus";
        const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileFormat;
        const string FileFormat = "tici";
        const string DefaultTitle = "untitled";
        const string Unsaved = "\u2022";
        const double ScaleFactor = 0.9;
        const int UndoBufferSize = 32;
        #endregion

        #region Properties
        Point lastMousePos;
        Point lastMouseClick;

        List<Gate> selected;
        SimulationContext context;
        List<Gate> gates = new List<Gate>();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Title = WindowTitle;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                context = Storage.Load(args[1]);
            else
                context = new SimulationContext();
            foreach (Gate gate in context.Gates)
                gate.Renderer.Render();

            var gate0 = new Gate();
            gate0.Renderer = new SimpleGateRenderer(this.canvas, gate0);
            context.Add(gate0);
            var gate1 = new Gate();
            gate1.output.Add(new Output());
            gate1.input.Add(new Input());
            gate1.input.Add(new Input());
            gate1.Renderer = new SimpleGateRenderer(this.canvas, gate1);
            gate1.Position = new Point(40, 0);
            context.Add(gate1);
            gate1.Position = new Point(40, 20);
            context.Remove(gate0);

            var g0 = new Gate();
            var g1 = new Gate();
            var g2 = new Gate();
            var g3 = new Gate();

            gates.Add(g0);
            gates.Add(g1);
            gates.Add(g2);
            gates.Add(g3);

            g0.Position = new Point(1, 6);
            g1.Position = new Point(0, 0);
            g2.Position = new Point(8, 3);
            g3.Position = new Point(8, 9);

            var eg0 = new OrElementaryGate();
            var eg1 = new NotElementaryGate();
            var eg2 = new AndElementaryGate();
            var eg3 = new OrElementaryGate();
            var eg4 = new AndElementaryGate();

            eg0.ConnectTo(eg1);
            eg0.ConnectTo(eg2);
            eg0.ConnectTo(eg3);
            eg0.ConnectTo(eg4);
            eg1.ConnectTo(eg2);

            var g0in0 = new Input();
            var g0in1 = new Input();
            var g0out0 = new Output();
            var g1in0 = new Input();
            var g1out0 = new Output();
            var g2in0 = new Input();
            var g2in1 = new Input();
            var g2out0 = new Output();
            var g3in0 = new Input();
            var g3in1 = new Input();
            var g3out0 = new Output();
            var g3out1 = new Output();

            g0.input.Add(g0in0);
            g0.input.Add(g0in1);
            g0.output.Add(g0out0);
            g1.input.Add(g1in0);
            g1.output.Add(g1out0);
            g2.input.Add(g2in0);
            g2.input.Add(g2in1);
            g2.output.Add(g2out0);
            g3.input.Add(g3in0);
            g3.input.Add(g3in1);
            g3.output.Add(g3out0);
            g3.output.Add(g3out1);

            g1out0.inverted = true;

            g0.tag = ">=1";
            g1.tag = "1";
            g2.tag = "&";
            g3.tag = "Custom";

            g0.mutable = true;
            g1.mutable = false;
            g2.mutable = true;
            g3.mutable = false;

            g3in0.name = "a";
            g3in1.name = "b";
            g3out0.name = "c";
            g3out1.name = "d";
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
            lastMousePos = lastMouseClick = e.GetPosition(canvas);
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
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
