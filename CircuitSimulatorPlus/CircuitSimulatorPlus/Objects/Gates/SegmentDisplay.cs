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
        
        public override int MinAmtInputNodes
        {
            get
            {
                return 7;
            }
        }
        public override int MaxAmtInputNodes
        {
            get
            {
                return 7;
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
            CreateConnectionNodes(7, 0);
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }

        public override void Add(bool addNodes = true)
        {
            new SegmentDisplayRenderer(this);
            base.Add(addNodes);
        }
    }
}
