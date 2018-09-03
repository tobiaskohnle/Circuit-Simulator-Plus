using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OrGate : Gate
    {
        public OrGate()
        {
            Size = new Size(3, 4);
            Tag = "\u22651";
        }

        public override void CreateDefaultConnectionNodes()
        {
            CreateConnectionNodes(2, 1);
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                if (input.LogicState)
                    return true;
            return false;
        }
    }
}
