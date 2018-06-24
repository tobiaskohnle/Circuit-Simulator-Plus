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
    public class CableRenderer
    {
        Canvas canvas;
        Cable cable;
        List<Point> points;
        List<Line> lines = new List<Line>();
        bool lastSegmentHorizontal;

        InputNode inputNode;
        OutputNode outputNode;

        public CableRenderer(Canvas canvas, Cable cable, List<Point> points, InputNode inputNode, OutputNode outputNode)
        {
            this.canvas = canvas;
            this.cable = cable;
            this.points = points;
            points.Add(outputNode.Position);
            points.Add(inputNode.Position);

            Render();
        }

        public void Render()
        {
            for (int i = 1; i < points.Count; i++)
            {
                Point pos1 = points[i - 1];
                Point pos2 = points[i];
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = pos1.X;
                line.Y1 = pos1.Y;
                line.X2 = pos2.X;
                line.Y2 = pos2.Y;
                canvas.Children.Add(line);
                lines.Add(line);
            }
        }

        public void Unrender()
        {
            foreach (Line line in lines)
                canvas.Children.Remove(line);
        }

        public void OnConnectedNodesPositionChanged()
        {
        }

        public void OnSelectionChanged()
        {
        }

        public void OnPointsChanged()
        {
        }
    }
}
