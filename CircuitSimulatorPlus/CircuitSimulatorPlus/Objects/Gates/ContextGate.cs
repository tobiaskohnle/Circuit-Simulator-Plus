﻿using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class ContextGate : Gate
    {
        public ContextGate() : base(0, 0)
        {
        }

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
