using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public OutputNode(Gate owner) : base(owner)
        {
        }

        /// <summary>
        /// True, if this OutputNode has a master-slave symbol next to it.
        /// </summary>
        public bool IsMasterSlave { get; set; }
        /// <summary>
        /// </summary>
        /// <param name="inputNode"></param>
        public void ConnectTo(InputNode inputNode)
        {
            NextConnectedTo.Add(inputNode);
            inputNode.BackConnectedTo = this;
            IsEmpty = inputNode.IsEmpty = false;
        }
        /// <summary>
        /// Clears this OutputNode.
        /// </summary>
        public override void Clear()
        {
            foreach (InputNode input in NextConnectedTo.ToList())
                input.Clear();
        }
    }
}
