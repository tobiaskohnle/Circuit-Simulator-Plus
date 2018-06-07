
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus.Rendering
{
    class OutputlightsRenderer
    {
        bool Output;
        double Width, Height;
        Line line;
        Rectangle Rect;
        List<Line> connectionlines;


        public void Render(Canvas canvas, double X1, double X2, double Y1, double Y2)
        {
            line = new Line();
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = X1;
            line.X2 = X1 - 5;
            line.Y1 = (Y1 + Y2) / 2;
            line.Y2 = Y1+(Y1 - Y2) / 2;
            connectionlines.Add(line);
            canvas.Children.Add(line);
            Rect = new Rectangle();
            Rect.StrokeThickness = MainWindow.LineWidth;
            Rect.Width = X2 - X1;
            Rect.Height = Y2 - Y1;
            canvas.Children.Add(Rect);
        }
        public void OnInputChanged(object sender, EventArgs e)
        {
            Brush brush = Output ? Brushes.Red : Brushes.Black;
            foreach (Line line in connectionlines)
            {
                line.Stroke = brush;
            }
        }

    }
}
