using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputSwitch : Gate
    {
        public InputSwitch()
        {
            Size = new Size(2, 2);
        }

        public override int MaxAmtInputNodes
        {
            get
            {
                return 0;
            }
        }
        public override int MinAmtOutputNodes
        {
            get
            {
                return 1;
            }
        }
        public override int MaxAmtOutputNodes
        {
            get
            {
                return 1;
            }
        }

        public override void CreateDefaultConnectionNodes()
        {
            CreateConnectionNodes(0, 1);
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
            State = !State;
        }

        public override void Add()
        {
            new InputSwitchRenderer(this);
            base.Add();
        }
    }
}
