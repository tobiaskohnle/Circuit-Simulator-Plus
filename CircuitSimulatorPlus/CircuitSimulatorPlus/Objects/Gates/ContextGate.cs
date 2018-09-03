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

        public override bool HasContext
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

        // TODO
        public void ReconnectInnerInputs()
        {

        }

        // TODO
        public void ReconnectInnerOutputs()
        {

        }

        // These are used for positional comparisons.
        public List<InputSwitch> InputSwitches = new List<InputSwitch>();
        public List<OutputLight> OutputLights = new List<OutputLight>();
    }
}
