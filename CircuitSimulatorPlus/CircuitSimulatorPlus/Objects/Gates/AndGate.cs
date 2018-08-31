using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class AndGate : Gate
    {
        public AndGate() : base(2, 1)
        {
            Tag = "&";
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                if (input.LogicState == false)
                    return false;
            return true;
        }
    }
}
