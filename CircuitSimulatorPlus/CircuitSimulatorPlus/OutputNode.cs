using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        protected bool masterSlave;

        public void ConnectTo(InputNode inputNode)
        {
        }

        public override void Clear()
        {
        }
    }
}
