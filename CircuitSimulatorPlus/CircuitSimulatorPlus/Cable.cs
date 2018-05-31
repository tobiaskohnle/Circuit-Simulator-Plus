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
        public List<Point> Points = new List<Point>();
        public CableRenderer Renderer;
        public OutputNode Output;
        public InputNode Input;

        bool lastSegmentHorizontal = true;

        public void AddPoint(Point point, bool toConnection = false)
        {
            if (Points.Count > 0)
            {
                Point lastPoint = Points.Last();
                if (point.X != lastPoint.X && point.Y != lastPoint.Y)
                {
                    bool horizontalWouldOverlap = false, verticalWouldOverlap = false;
                    if (Points.Count > 1)
                    {
                        Point lastLastPoint = Points[Points.Count - 2];
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
                        Points.Add(new Point(midX, lastPoint.Y));
                        Points.Add(new Point(midX, point.Y));
                    }
                    else
                    {
                        Point cornerPoint;
                        if (verticalWouldOverlap || (lastSegmentHorizontal && !horizontalWouldOverlap))
                            cornerPoint = new Point(point.X, lastPoint.Y);
                        else
                            cornerPoint = new Point(lastPoint.X, point.Y);
                        Points.Add(cornerPoint); 
                    }
                }
                lastSegmentHorizontal = point.Y == Points.Last().Y;
            }
            Points.Add(point);
            if (Renderer != null)
                Renderer.Update();
        }
    }
}
