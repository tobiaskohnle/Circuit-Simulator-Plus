﻿using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableSegmentHitbox : LineHitbox
    {
        CableSegment cableSegment;

        public CableSegmentHitbox(CableSegment cableSegment) : base(Cable.HitboxWidth, Cable.DistanceFactor)
        {
            this.cableSegment = cableSegment;
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
