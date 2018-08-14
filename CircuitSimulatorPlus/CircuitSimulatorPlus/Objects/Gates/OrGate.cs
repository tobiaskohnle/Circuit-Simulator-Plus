using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class OrGate : Gate
    {
        public OrGate() : base(2, 1)
        {
            Tag = "\u22651";
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                return input.State;
            return false;
        }
    }
}
