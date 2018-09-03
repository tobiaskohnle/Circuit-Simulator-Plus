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

        public event Action OnStateChanged;
        bool state;
        public bool State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnStateChanged?.Invoke();
            }
        }

        public override bool Eval()
        {
            return State;
        }

        public void Toggle()
        {
            state = !state;
        }
    }
}
