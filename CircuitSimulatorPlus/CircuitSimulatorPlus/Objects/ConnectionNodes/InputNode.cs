using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(owner)
        {
            new ConnectionNodeRenderer(this, owner, false);
            new InputNodeRenderer(this);
        }

        public event Action OnRisingEdgeChanged;
        bool isRisingEdge;
        /// <summary>
        /// True, if this InputNode reacts to rising edges.
        /// </summary>
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

        public override bool LogicState
        {
            get
            {
                if (IsRisingEdge)
                {
                    return State && stateChanged;
                }
                return State;
            }
        }

        /// <summary>
        /// True, if this InputNode is displayed in the center
        /// of a gate, independent of other InputsNodes.
        /// </summary>
        public bool IsCentered
        {
            get; set;
        }
        /// <summary>
        /// Clears this InputNode.
        /// </summary>
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

            MainWindow.Self.Tick(this);
        }

        public override void Tick()
        {
            Tick(false, !Owner.HasContext, IsRisingEdge && LogicState);
            if (IsRisingEdge && LogicState)
            {
                MainWindow.Self.Tick(this);
            }
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
    }
}
