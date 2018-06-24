using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class OrGate : Gate
    {
        public OrGate()
        {
            Tag = "\u22651";
        }

        public override string Type
        {
            get
            {
                return "Or";
            }
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                return input.State;
            return false;
        }
    }
}
