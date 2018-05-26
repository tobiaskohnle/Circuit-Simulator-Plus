using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        protected bool risingEdge;
        protected bool centered;

        public void ConnectTo(OutputNode outputNode)
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
