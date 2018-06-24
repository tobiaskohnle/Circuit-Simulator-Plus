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

        public void OnLayoutChanged()
        {
            Brush brush = cable.OutputNode.State ? Brushes.Red : Brushes.Black;
            foreach (Line line in lines)
            {
                line.Stroke = brush;
            }

            Gate outputGate = outputNode.Owner;
            int outputIndex = 0;
            for (int i = 0; i < outputGate.Output.Count; i++)
            {
                if (outputGate.Output[i] == outputNode)
                {
                    outputIndex = i;
                    break;
                }
            }
            Point inputPos = points.Last();
            var outputPos = new Point(outputGate.Position.X + 3 + 1, outputGate.Position.Y + (double)4 * (1 + 2 * outputIndex) / (2 * outputGate.Output.Count));
            points.Clear();
            lastSegmentHorizontal = true;
            addPoint(outputPos, true);
            addPoint(inputPos, true);


            Gate inputGate = inputNode.Owner;
            int inputIndex = 0;
            for (int i = 0; i < inputGate.Input.Count; i++)
            {
                if (inputGate.Input[i] == inputNode)
                {
                    inputIndex = i;
                    break;
                }
            }
        }

        void addPoint(Point point, bool toConnection = false)
        {
            if (points.Count > 0)
            {
                Point lastPoint = points.Last();
                if (point.X != lastPoint.X && point.Y != lastPoint.Y)
                {
                    bool horizontalWouldOverlap = false, verticalWouldOverlap = false;
                    if (points.Count > 1)
                    {
                        Point lastLastPoint = points[points.Count - 2];
                        if (lastSegmentHorizontal)
                        {
                            if (lastLastPoint.X < lastPoint.X)
                                horizontalWouldOverlap = point.X < lastPoint.X;
                            else
                                horizontalWouldOverlap = point.X > lastPoint.X;
                        }
                        else
                        {
                            if (lastLastPoint.Y < lastPoint.Y)
                                verticalWouldOverlap = point.Y < lastPoint.Y;
                            else
                                verticalWouldOverlap = point.Y > lastPoint.Y;
                        }
                    }

                    if (toConnection && lastSegmentHorizontal && !horizontalWouldOverlap)
                    {
                        double midX = (lastPoint.X + point.X) / 2;
                        points.Add(new Point(midX, lastPoint.Y));
                        points.Add(new Point(midX, point.Y));
                    }
                    else
                    {
                        Point cornerPoint;
                        if (verticalWouldOverlap || (lastSegmentHorizontal && !horizontalWouldOverlap))
                            cornerPoint = new Point(point.X, lastPoint.Y);
                        else
                            cornerPoint = new Point(lastPoint.X, point.Y);
                        points.Add(cornerPoint);
                    }
                }
                lastSegmentHorizontal = point.Y == points.Last().Y;
            }
            points.Add(point);
        }
    }
}
