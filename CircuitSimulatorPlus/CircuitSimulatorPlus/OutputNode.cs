using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public bool masterSlave;

        List<InputNode> connectedTo;

        public void ConnectTo(InputNode inputNode)
        {
            if (empty == false)
                return;
            empty = false;

            connectedTo.Add(inputNode);

            repr.ConnectTo(inputNode.repr);

            if (inputNode.IsEmpty)
                inputNode.ConnectTo(this);
        }

        public override void Clear()
        {
            if (empty)
                return;
            empty = true;

            connectedTo.Clear();
        }
    }
}
