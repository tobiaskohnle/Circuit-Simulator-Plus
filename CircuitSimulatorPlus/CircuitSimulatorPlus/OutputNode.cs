using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        protected bool masterSlave;

        protected List<InputNode> connectedTo = new List<InputNode>();

        public void ConnectTo(InputNode inputNode)
        {
        }

        public override void Clear()
        {
        }

        public override void Invert()
        {
            if (inverted)
            {
            }
            else
            {
            }
            inverted = !inverted;
        }
    }
}
