using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class NopGate : Gate
    {
        public NopGate()
        {
            Tag = "1";
        }

        public override string Type
        {
            get
            {
                return "Nop";
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
