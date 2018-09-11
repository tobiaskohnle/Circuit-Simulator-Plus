﻿using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class InputButton : Gate
    {
        public InputButton()
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

        public override void Add(bool addNodes = true)
        {
            new InputButtonRenderer(this);
            base.Add(addNodes);
        }
    }
}
