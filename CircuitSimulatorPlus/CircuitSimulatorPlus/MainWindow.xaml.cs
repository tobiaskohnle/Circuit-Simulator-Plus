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
        #region Contstans
        const string Title = "Circuit Simulator Plus";
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
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            base.Title = Title;
        }

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
    }
}
