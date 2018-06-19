using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class InputSwitch : Gate
    {
        public override string Type
        {
            get {
                return "InputSwitch";
            }
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }
    }
}
