using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OutputLight : Gate
    {
        public OutputLight() : base(1, 0)
        {
            Size = new Size(2, 2);
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }
    }
}
