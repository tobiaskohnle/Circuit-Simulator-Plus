using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputSwitch : Gate
    {
        public InputSwitch() : base(0, 1)
        {
            new InputSwitchRenderer(this);
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
    }
}
