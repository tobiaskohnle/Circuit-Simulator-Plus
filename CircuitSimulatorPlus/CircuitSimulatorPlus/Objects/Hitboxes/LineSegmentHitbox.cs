using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class LineSegmentHitbox : LineHitbox
    {
        CableSegment cableSegment;

        public LineSegmentHitbox(CableSegment cableSegment) : base(Cable.HitboxWidth, Cable.DistanceFactor)
        {
            this.cableSegment = cableSegment;
            UpdateHitbox();
        }

        public override void UpdateHitbox()
        {
            Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
            Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
            Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

            vert = (cableSegment.Index & 1) != 0;

            x = vert ? lastPoint.Y : lastPoint.X;
            y = vert ? point.X : point.Y;
            z = vert ? nextPoint.Y : nextPoint.X;
        }
    }
}
