using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputSwitch : Gate
    {
        public InputSwitch()
        {
            Size = new Size(2, 2);
        }
        public bool State
        {
            get; set;
        }
        public void color()
        {
            
        }
        public override bool Eval()
        {
            return State;
        }
    }
}
