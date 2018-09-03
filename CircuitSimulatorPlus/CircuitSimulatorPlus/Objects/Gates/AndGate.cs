using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class AndGate : Gate
    {
        public AndGate()
        {
            Size = new Size(3, 4);
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
