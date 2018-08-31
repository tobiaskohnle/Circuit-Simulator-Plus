using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OutputNode : ConnectionNode
    {
        public OutputNode(Gate owner) : base(owner)
        {
            new ConnectionNodeRenderer(this, owner, true);
            new OutputNodeRenderer(this);
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
            //ConnectionNode localNode = this;
            //ConnectionNode remoteNode = connectionNode;

            //if (Owner is InputSwitch)
            //{
            //    InputSwitch inputSwitch = (InputSwitch)Owner;
            //    ContextGate context = inputSwitch.Parent;
            //    if (context == null)
            //        throw new Exception("InputSwitch has no parent");
            //    localNode = context.Input[inputSwitch.Index];
            //}
            //if (connectionNode.Owner is OutputLight)
            //{
            //    OutputLight outputLight = (OutputLight)connectionNode.Owner;
            //    ContextGate context = outputLight.Parent;
            //    if (context == null)
            //        throw new Exception("OutputLight has no parent");
            //    remoteNode = context.Output[outputLight.Index];
            //}

            //localNode.NextConnectedTo.Add(remoteNode);
            //remoteNode.BackConnectedTo = this;

            if (!connectionNode.IsEmpty)
            {
                connectionNode.Clear();
            }

            NextConnectedTo.Add(connectionNode);
            connectionNode.BackConnectedTo = this;

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

        public override void Tick()
        {
            Tick(!Owner.HasContext, false, false);
        }

        public override void UpdatePosition(int index)
        {
            Position = new Point(
                Owner.Position.X + Owner.Size.Width,
                Owner.Position.Y + Owner.Size.Height * (1 + 2 * index) / (2 * Owner.Output.Count)
            );
        }
    }
}
