using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(Align.L, owner)
        {
        }

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
                Renderer.OnRisingEdgeChanged();
            }
        }

        /// <summary>
        /// True, if this InputNode is displayed in the center.
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
                return;
            BackConnectedTo.NextConnectedTo.Remove(this);
            BackConnectedTo.IsEmpty = BackConnectedTo.NextConnectedTo.Count == 0;
            BackConnectedTo = null;
            IsEmpty = true;
            Owner.Renderer.OnPositionChanged();
        }

        public override void Tick(Queue<ConnectionNode> tickedNodes)
        {
            if (IsRisingEdge)
            {
                if (State == false)
                    State = BackConnectedTo.State;
                if (State)
                    tickedNodes.Enqueue(this);
                foreach (ConnectionNode node in NextConnectedTo)
                    tickedNodes.Enqueue(node);
            }
            else
            {
                Tick(tickedNodes, !Owner.HasContext, false);
            }
        }

        public override void ConnectTo(ConnectionNode connectionNode)
        {
            connectionNode.NextConnectedTo.Add(this);
            BackConnectedTo = connectionNode;
        }
    }
}
