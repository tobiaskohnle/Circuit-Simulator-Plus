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

        public List<Cable> ConnectedCables = new List<Cable>();

        bool isMasterSlave;

        public event Action OnMasterSlaveChanged;
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

        public override Point CableAnchorPoint
        {
            get
            {
                return new Point(Position.X + 1, Position.Y);
            }
        }

        public override void ConnectTo(ConnectionNode connectionNode)
        {
            if (!connectionNode.IsEmpty)
            {
                connectionNode.Clear();
            }

            NextConnectedTo.Add(connectionNode);
            connectionNode.BackConnectedTo = this;

            IsEmpty = connectionNode.IsEmpty = false;

            MainWindow.Self.Tick(this);
        }

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

        public override void Add()
        {
            new ConnectionNodeRenderer(this, Owner, true);
            new OutputNodeRenderer(this);
            IsRendered = true;
            base.Add();
        }
    }
}
