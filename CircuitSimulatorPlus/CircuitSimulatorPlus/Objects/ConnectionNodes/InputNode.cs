using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(owner)
        {
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
                    return State && ticksActive < 3;
                }
                return State;
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
            BackConnectedTo = connectionNode;
            connectionNode.NextConnectedTo.Add(this);

            IsEmpty = connectionNode.IsEmpty = false;
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
