using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public bool risingEdge;
        public bool centered;

        OutputNode connectedTo;

        public void ConnectTo(OutputNode outputNode)
        {
            if (empty == false)
                return;
            empty = false;

            connectedTo = outputNode;

            if (outputNode.IsEmpty)
                outputNode.ConnectTo(this);
        }

        public override void Clear()
        {
            if (empty)
                return;
            empty = true;

            connectedTo = null;
        }
    }
}
