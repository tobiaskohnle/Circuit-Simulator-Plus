using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class NopGate : Gate
    {
        public NopGate() : base(1, 1)
        {
            Tag = "1";
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                return input.LogicState;
            return false;
        }
    }
}
