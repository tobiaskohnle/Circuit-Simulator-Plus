using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(owner)
        {
        }

        public void CopyFrom(InputNode inputNode)
        {
            NextConnectedTo = inputNode.NextConnectedTo;
            foreach (ConnectionNode node in inputNode.NextConnectedTo)
                node.BackConnectedTo = this;

            IsRisingEdge = inputNode.IsRisingEdge;
            base.CopyFrom(inputNode);
        }

        public Cable ConnectedCable;

        public event Action OnRisingEdgeChanged;
        bool isRisingEdge;
        public bool IsRisingEdge
        {
            get
            {
                return isRisingEdge;
            }
            set
            {
                isRisingEdge = value;
                MainWindow.Self.Tick(this);
                OnRisingEdgeChanged?.Invoke();
            }
        }

        int ticksActive;

        public override bool LogicState
        {
            get
            {
                if (IsRisingEdge)
                {
                    return State && ticksActive < Constants.RisingEdgePulse;
                }
                return State;
            }
        }

        public override Point CableAnchorPoint
        {
            get
            {
                return new Point(Position.X - 1, Position.Y);
            }
        }

        public override void Clear()
        {
            if (IsEmpty)
            {
                return;
            }

            BackConnectedTo.NextConnectedTo.Remove(this);
            BackConnectedTo.IsEmpty = BackConnectedTo.NextConnectedTo.Count == 0;
            BackConnectedTo = null;
            IsEmpty = true;

            ConnectedCable.Remove();

            MainWindow.Self.Tick(this);
        }

        public override void Tick()
        {
            Tick(false, !Owner.HasContext, IsRisingEdge && LogicState);

            if (IsRisingEdge && LogicState)
                MainWindow.Self.Tick(this);

            if (State)
                ticksActive++;
            else
                ticksActive = 0;
        }

        public override void ConnectTo(ConnectionNode connectionNode)
        {
            if (!IsEmpty)
            {
                Clear();
            }

            connectionNode.NextConnectedTo.Add(this);
            BackConnectedTo = connectionNode;

            IsEmpty = connectionNode.IsEmpty = false;

            MainWindow.Self.Tick(this);
            MainWindow.Self.Tick(connectionNode);
        }

        public override void UpdatePosition(int index)
        {
            Position = new Point(
                Owner.Position.X,
                Owner.Position.Y + Owner.Size.Height * (1 + 2 * index) / (2 * Owner.Input.Count)
            );
        }

        public override void Add()
        {
            new ConnectionNodeRenderer(this, Owner, false);
            new InputNodeRenderer(this);
            IsRendered = true;
            base.Add();
        }
    }
}
