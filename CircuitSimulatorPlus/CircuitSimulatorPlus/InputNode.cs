using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class InputNode : ConnectionNode
    {
        public InputNode(Gate owner) : base(owner)
        {
        }

        /// <summary>
        /// True, if this InputNode reacts to rising edges.
        /// </summary>
        public bool IsRisingEdge { get; set; }
        /// <summary>
        /// True, if this InputNode is displayed in the center.
        /// of a gate, independent of other InputsNodes.
        /// </summary>
        public bool IsCentered { get; set; }
        /// <summary>
        /// Clears this InputNode.
        /// </summary>
        public override void Clear()
        {
            BackConnectedTo.NextConnectedTo.Remove(this);
            BackConnectedTo.IsEmpty = BackConnectedTo.NextConnectedTo.Count == 0;
            BackConnectedTo = null;
            IsEmpty = true;
        }
    }
}
