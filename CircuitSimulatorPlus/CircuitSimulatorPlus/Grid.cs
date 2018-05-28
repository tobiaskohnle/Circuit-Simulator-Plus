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
    class Grid
    {
        Canvas canvas;
        int width, height;
        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>();

        public Grid(Canvas canvas, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.canvas = canvas;
        }
        public void Draw()
        {
            for (int i = 0; i < width; i += 2)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = i;
                line.X2 = i;
                line.Y1 = 0;
                line.Y2 = height;
                verticalLines.Add(line);
                canvas.Children.Add(line);
            }
            for (int i = 0; i < height; i += 2)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = 0;
                line.X2 = width;
                line.Y1 = i;
                line.Y2 = i;
                verticalLines.Add(line);
                canvas.Children.Add(line);
            }
        }
    }
}
