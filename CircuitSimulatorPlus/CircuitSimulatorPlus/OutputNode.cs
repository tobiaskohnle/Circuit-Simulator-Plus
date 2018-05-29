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
            Owner.ConnectionCreated?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Clears this OutputNode.
        /// </summary>
        public override void Clear()
        {
            foreach (InputNode input in NextConnectedTo.ToList())
                input.Clear();
        }

        public override void Tick(Queue<ConnectionNode> tickedNodes)
        {
            Tick(tickedNodes, true);
        }
    }
}
