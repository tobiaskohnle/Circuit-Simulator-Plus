using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public OutputNode(Gate owner) : base(Align.R, owner)
        {
            new ConnectionNodeRenderer(this, owner, true);
        }

        bool isMasterSlave = true;

        public event Action OnMasterSlaveChanged;
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
            ConnectionNode localNode = this;
            ConnectionNode remoteNode = connectionNode;

            if (Owner is InputSwitch)
            {
                InputSwitch inputSwitch = (InputSwitch)Owner;
                ContextGate context = inputSwitch.Parent;
                if (context == null)
                    throw new Exception("InputSwitch has no parent");
                localNode = context.Input[inputSwitch.Index];
            }
            if (connectionNode.Owner is OutputLight)
            {
                OutputLight outputLight = (OutputLight)connectionNode.Owner;
                ContextGate context = outputLight.Parent;
                if (context == null)
                    throw new Exception("OutputLight has no parent");
                remoteNode = context.Output[outputLight.Index];
            }

            localNode.NextConnectedTo.Add(remoteNode);
            remoteNode.BackConnectedTo = this;

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
