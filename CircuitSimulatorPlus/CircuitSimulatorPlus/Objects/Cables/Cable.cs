using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Cable(InputNode inputNode, OutputNode outputNode)
        {
            InputNode = inputNode;
            OutputNode = outputNode;
            Renderer = new CableRenderer(this);
        }

        public CableRenderer Renderer;

        public OutputNode OutputNode
        {
            get; set;
        }

        public InputNode InputNode
        {
            get; set;
        }

        public void CreateCable()
        {
        }

        public void DeleteCable()
        {
        }
    }
}
