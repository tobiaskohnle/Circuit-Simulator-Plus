using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class ContextGate : Gate
    {
        /// <summary>
        /// The circuit inside of a gate.
        /// </summary>
        public List<Gate> Context { get; set; } = new List<Gate>();

        public new bool HasContext
        {
            get
            {
                return true;
            }
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }
    }
}
