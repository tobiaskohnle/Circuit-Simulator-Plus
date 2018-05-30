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

        public void AddPoint(Point point)
        {
            if (Points.Count > 0)
                Points.Add(new Point(Points.Last().X, point.Y));
            Points.Add(point);
            if (Renderer != null)
                Renderer.Update();
        }
    }
}
