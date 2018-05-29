using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    class Cabel
    {
        Canvas canvas;
        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>();
        double positionX1, positionX2, positionY1, positionY2;
        public Cabel(Canvas canvas)
        {
            this.canvas = canvas;
        }
        public void Draw()
        {
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1;
            line.X2 = positionX1;
            line.Y1 = positionY1;
            line.Y2 = positionY2;
            verticalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1;
            line.X2 = positionX2/2;
            line.Y1 = positionY1;
            line.Y2 = positionY1;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX2/2;
            line.X2 = positionX2;
            line.Y1 = positionY2;
            line.Y2 = positionY2;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
        }
    }
}
