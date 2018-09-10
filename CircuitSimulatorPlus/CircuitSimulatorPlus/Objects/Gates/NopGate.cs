using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class NopGate : Gate
    {
        public NopGate()
        {
            Size = new Size(3, 4);
            Tag = "1";
        }

        public override int MinAmtInputNodes
        {
            get
            {
                return 1;
            }
        }
        public override int MaxAmtInputNodes
        {
            get
            {
                return 1;
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
            CreateConnectionNodes(1, 1);
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                return input.LogicState;
            return false;
        }
    }
}
