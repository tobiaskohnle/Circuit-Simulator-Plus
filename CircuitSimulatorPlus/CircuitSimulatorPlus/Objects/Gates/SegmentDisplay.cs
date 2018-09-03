using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class SegmentDisplay : Gate
    {
        public SegmentDisplay()
        {
            Size = new Size(5, 7);
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }

        public override void Add()
        {
            new SegmentDisplayRenderer(this);
            base.Add();
        }
    }
}
