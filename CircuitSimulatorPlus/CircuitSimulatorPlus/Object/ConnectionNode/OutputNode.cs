using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public OutputNode(Gate owner) : base(Align.E, owner)
        {
        }

        public bool isMasterSlave;

        public override void UpdateHitbox()
        {
        }

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
                Renderer.OnMasterSlaveChanged();
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="connectionNode"></param>
        public override void ConnectTo(ConnectionNode connectionNode)
        {
            base.ConnectTo(connectionNode);
            IsEmpty = connectionNode.IsEmpty = false;
            Owner.Renderer.OnPositionChanged();
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
