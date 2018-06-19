using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class OutputLight : Gate
    {
        public override string Type
        {
            get {
                return "OutputLight";
            }
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }
    }
}
