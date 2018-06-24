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
        public CableRenderer Renderer
        {
            get; set;
        }

        bool lastSegmentHorizontal = true;

        public OutputNode OutputNode
        {
            get; set;
        }

        public InputNode InputNode
        {
            get; set;
        }
    }
}
