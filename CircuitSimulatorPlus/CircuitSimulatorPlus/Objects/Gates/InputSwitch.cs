using CircuitSimulatorPlus.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputSwitch : Gate
    {
        public InputSwitch(ContextGate parent) : this()
        {
            Parent = parent;
            OnPositionChanged += SortInputs;
        }

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

        private ContextGate parent;
        public ContextGate Parent
        {
            set
            {
                if (parent != null)
                {
                    parent.InputSwitches.Remove(this);
                }
                parent = value;
                if (parent != null)
                {
                    parent.InputSwitches.Add(this);
                    SortInputs();
                }
            }
            get { return parent; }
        }

        public int Index
        {
            get
            {
                if (Parent == null)
                    throw new Exception("InputSwitch has no parent");
                return Parent.InputSwitches.BinarySearch(this, GatePositionComparer.Instance);
            }
        }

        private void SortInputs()
        {
            Parent.InputSwitches.Sort(GatePositionComparer.Instance);
        }
    }
}
