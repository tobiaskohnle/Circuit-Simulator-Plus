using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    class OutputLightRenderer
    {
        bool isOutput;
        double width, height;
        Line line;
        Rectangle rect;
        List<Line> connectionLines;

        public void Render(Canvas canvas, double x1, double x2, double y1, double y2)
        {
            line = new Line();
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = x1;
            line.X2 = x1 - 5;
            line.Y1 = (y1 + y2) / 2;
            line.Y2 = y1 + (y1 - y2) / 2;
            connectionLines.Add(line);
            canvas.Children.Add(line);
            rect = new Rectangle();
            rect.StrokeThickness = MainWindow.LineWidth;
            rect.Width = x2 - x1;
            rect.Height = y2 - y1;
            canvas.Children.Add(rect);
        }
        public void OnInputChanged(object sender, EventArgs e)
        {
            Brush brush = isOutput ? Brushes.Red : Brushes.Black;
            foreach (Line line in connectionLines)
            {
                line.Stroke = brush;
            }
        }

    }
}
