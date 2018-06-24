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
        InputNode inputNode;
        OutputNode outputNode;
        List<Point> points;
        RectHitbox hitbox;
        bool isSelected;

        public CableRenderer Renderer
        {
            get; set;
        }

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
