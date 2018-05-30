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
    public class Cable
    {
        /*Canvas canvas;
        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>();
        double positionX1, positionX2, positionY1, positionY2;
        public Cable(Canvas canvas, double positionX1, double positionX2, double positionY1, double positionY2)
        {
            this.positionX1 = positionX1;
            this.positionX2 = positionX2;
            this.positionY1 = positionY1;
            this.positionY2 = positionY2;
            this.canvas = canvas;
        }
        //TODO: Cable by drawing fails, if value is bigger as or is negative. 
        public void Draw()
        {
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1+(positionX2 - positionX1) / 2;
            line.X2 = positionX1+(positionX2 - positionX1)/2;
            line.Y1 = positionY1 -  MainWindow.LineWidth/2;
            line.Y2 = positionY2 +  MainWindow.LineWidth/2;
            verticalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1;
            line.X2 = positionX1 + 1 / 2 * MainWindow.LineWidth+(positionX2 - positionX1) / 2 ;
            line.Y1 = positionY1;
            line.Y2 = positionY1;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1 - 1 / 2 * MainWindow.LineWidth+(positionX2 - positionX1) / 2;
            line.X2 = positionX2;
            line.Y1 = positionY2;
            line.Y2 = positionY2;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
        }
        */
        public List<Point> points= new List<Point>();
        public CableRenderer Renderer;
        public Cable(Point point)
        {
            if(Renderer!=null)Renderer.Update();
        }
        public void create_points(Point point)
        {
            points.Add(point);
            if(Renderer!=null)Renderer.Update();
        }
    }
}
