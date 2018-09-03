using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class SplitSegmentCommand : Command
    {
        List<CableSegment> cableSegments;

        public SplitSegmentCommand(List<CableSegment> cableSegments) : base("Split Cable Segments")
        {
            this.cableSegments = cableSegments;
        }

        public override void Redo()
        {
            foreach (CableSegment segment in cableSegments)
            {
                segment.Parent.SplitSegment(segment.Index);
            }
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
