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
        double width, height, Linewidth = MainWindow.LineWidth;
        public double scale;
        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>();

        public Grid(Canvas canvas, double width, double height, double scale)
        {
            this.scale = scale;
            this.width = width;
            this.height = height;
            this.canvas = canvas;
        }
        public void Render()
        {
            scale = scale * 2;
            for (int i = 0; i < width; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = i;
                line.X2 = i;
                line.Y1 = 0;
                line.Y2 = height;
                horizontalLines.Add(line);
                canvas.Children.Add(line);
            }
            for (int i = 0; i < height; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = 0;
                line.X2 = width;
                line.Y1 = i;
                line.Y2 = i;
                verticalLines.Add(line);
                canvas.Children.Add(line);
            }
        }
        public void Gridlinewidth()
        {
            Linewidth = Linewidth / scale;
            foreach (Line line in verticalLines)
                line.StrokeThickness = Linewidth;
            foreach (Line line in horizontalLines)
                line.StrokeThickness = Linewidth;
        }
        /*public void position(double movedX, double movedY)
        {
            foreach (Line line in verticalLines)
            {
                line.X1 = line.X1 + movedX;
                line.X2 = line.X2 + movedX;
                line.Y1 = line.Y1 + movedY;
                line.Y1 = line.Y2 + movedY;
            }
            foreach (Line line in horizontalLines)
            {
                line.X1 = line.X1 + movedX;
                line.X2 = line.X2 + movedX;
                line.Y1 = line.Y1 + movedY;
                line.Y1 = line.Y2 + movedY;
            }
        }*/
    }
}
