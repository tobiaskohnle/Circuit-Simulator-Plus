using CircuitSimulatorPlus.Rendering;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OutputLight : Gate
    {
        public OutputLight()
        {
            Size = new Size(2, 2);
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
        public override int MaxAmtOutputNodes
        {
            get
            {
                return 0;
            }
        }

        public override void CreateDefaultConnectionNodes()
        {
            CreateConnectionNodes(1, 0);
        }

        public bool State
        {
            get; internal set;
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }

        public override void Add()
        {
            new OutputLightRenderer(this);
            base.Add();
        }
    }
}
