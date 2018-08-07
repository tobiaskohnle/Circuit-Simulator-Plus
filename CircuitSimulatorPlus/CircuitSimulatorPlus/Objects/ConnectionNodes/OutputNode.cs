using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public OutputNode(Gate owner) : base(Align.R, owner)
        {
            Renderer = new ConnectionNodeRenderer(this, owner, true);
        }

        bool isMasterSlave = true;

        public event System.Action OnMasterSlaveChanged;
        /// <summary>
        /// True, if this OutputNode has a master-slave symbol next to it.
        /// </summary>
        public bool IsMasterSlave
        {
            get
            {
                return isMasterSlave;
            }
            set
            {
                isMasterSlave = value;
                OnMasterSlaveChanged?.Invoke();
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="connectionNode"></param>
        public override void ConnectTo(ConnectionNode connectionNode)
        {
            NextConnectedTo.Add(connectionNode);
            connectionNode.BackConnectedTo = this;

            // TODO: clear connectionNode if !connectionNode.IsEmpty

            IsEmpty = connectionNode.IsEmpty = false;
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
            Tick(tickedNodes, false, !Owner.HasContext);
        }
    }
}
