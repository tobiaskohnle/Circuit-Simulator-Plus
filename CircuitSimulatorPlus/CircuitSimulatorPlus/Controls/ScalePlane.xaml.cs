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

namespace CircuitSimulatorPlus.Controls
{
    /// <summary>
    /// Interaktionslogik für ScalePlane.xaml
    /// </summary>
    public partial class ScalePlane : UserControl
    {
        const double StartScale = 20;
        const double LineThickness = 1;

        Pen backgroundGridPen;
        Brush backgroundGridBrush;
        double currentScale = StartScale;
        Matrix matrix = new ScaleTransform(StartScale, StartScale).Value;
        Point lastMousePos;
        bool dragging;

        public UIElementCollection Children
        {
            get { return canvas.Children; }
        }

        public ScalePlane()
        {
            InitializeComponent();

            backgroundGridPen = new Pen(Brushes.LightGray, LineThickness / currentScale);
            var geometry = new RectangleGeometry(new Rect(0, 0, 1, 1));
            var drawing = new GeometryDrawing(Brushes.White, backgroundGridPen, geometry);
            backgroundGridBrush = new DrawingBrush
            {
                Drawing = drawing,
                Viewport = new Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Stretch = Stretch.Fill
            };

            Background = backgroundGridBrush;
            UpdateFromMatrix();
        }


        void UpdateFromMatrix()
        {
            Transform transform = new MatrixTransform(matrix);
            canvas.RenderTransform = transform;
            backgroundGridBrush.Transform = transform;
            backgroundGridPen.Thickness = LineThickness / currentScale;
        }

        void ScaleAt(double scale, Point pos)
        {
            currentScale *= scale;
            matrix.ScaleAtPrepend(scale, scale, pos.X, pos.Y);
            UpdateFromMatrix();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
                return;
            lastMousePos = e.GetPosition(this);
            dragging = true;
            CaptureMouse();
            e.Handled = true;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging)
                return;
            Point pos = e.GetPosition(this);
            Vector delta = pos - lastMousePos;
            lastMousePos = pos;
            matrix.Translate(delta.X, delta.Y);
            UpdateFromMatrix();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            dragging = false;
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scale = e.Delta > 0 ? 1.1 : 1 / 1.1;
            Point pos = e.GetPosition(canvas);
            ScaleAt(scale, pos);
        }
    }
}
