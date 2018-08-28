using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(owner)
        {
            AlignmentVector = new Vector(-1, 0);
            new ConnectionNodeRenderer(this, owner, false);
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
                OnRisingEdgeChanged?.Invoke();
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
            if (IsRisingEdge)
            {
                if (State == false)
                    State = BackConnectedTo.State;
                if (State)
                    MainWindow.Self.TickedNodes.Enqueue(this);
                foreach (ConnectionNode node in NextConnectedTo)
                    MainWindow.Self.TickedNodes.Enqueue(node);
            }
            else
            {
                Tick(false, !Owner.HasContext);
            }
        }

        public override void ConnectTo(ConnectionNode connectionNode)
        {
            //ConnectionNode localNode = this;
            //ConnectionNode remoteNode = connectionNode;

            //if (connectionNode.Owner is InputSwitch)
            //{
            //    InputSwitch inputSwitch = (InputSwitch)Owner;
            //    ContextGate context = inputSwitch.Parent;
            //    if (context == null)
            //        throw new Exception("InputSwitch has no parent");
            //    remoteNode = context.Input[inputSwitch.Index];
            //}
            //if (Owner is OutputLight)
            //{
            //    OutputLight outputLight = (OutputLight)connectionNode.Owner;
            //    ContextGate context = outputLight.Parent;
            //    if (context == null)
            //        throw new Exception("OutputLight has no parent");
            //    localNode = context.Output[outputLight.Index];
            //}

            //remoteNode.NextConnectedTo.Add(localNode);
            //localNode.BackConnectedTo = connectionNode;

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
