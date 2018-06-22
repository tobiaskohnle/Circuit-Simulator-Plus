using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class AndGate : Gate
    {
        public AndGate()
        {
            Tag = "&";
        }

        public override string Type
        {
            get
            {
                return "And";
            }
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                if (input.State == false)
                    return false;
            return true;
        }
    }
}
