using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public bool masterSlave;

        public List<InputNode> connectedTo = new List<InputNode>();

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
            //if (empty)
            //    return;
            foreach (InputNode inputNode in connectedTo.ToList())
                inputNode.Clear();
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
