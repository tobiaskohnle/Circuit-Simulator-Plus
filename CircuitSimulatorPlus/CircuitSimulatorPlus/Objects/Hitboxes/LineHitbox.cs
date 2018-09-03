using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class LineHitbox : Hitbox
    {
        CableSegment cableSegment;

        public LineHitbox(CableSegment cableSegment)
        {
            this.cableSegment = cableSegment;
        }

        public override Rect RectBounds
        {
            get
            {
                Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
                Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
                Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

                if ((cableSegment.Index & 1) != 0)
                    return new Rect(point.X, Math.Min(lastPoint.Y, nextPoint.Y), 0, Math.Abs(nextPoint.Y - lastPoint.Y));
                return new Rect(Math.Min(lastPoint.X, nextPoint.X), point.Y, Math.Abs(nextPoint.X - lastPoint.X), 0);
            }
        }

        double Dist(Point pos)
        {
            Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
            Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
            Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

            bool vert = (cableSegment.Index & 1) != 0;
            double startX = vert ? point.X : lastPoint.X;
            double startY = vert ? lastPoint.Y : point.Y;
            double endX = vert ? point.X : nextPoint.X;
            double endY = vert ? nextPoint.Y : point.Y;

            double dist = vert ? Math.Abs(pos.X - startX) : Math.Abs(pos.Y - startY);

            double lastDist = vert ? Math.Abs(pos.Y - startY) : Math.Abs(pos.X - startX);
            double nextDist = vert ? Math.Abs(pos.Y - endY) : Math.Abs(pos.X - endX);

            double len = vert ? Math.Abs(endY - startY) : Math.Abs(endX - startX);

            if (lastDist < len && nextDist < len)
                return dist - 1e-7;

            double minDist = Math.Min(lastDist, nextDist);
            double sideDist = Math.Max(dist, minDist);

            if (dist > minDist)
                return sideDist - 1e-7;

            return sideDist;
        }

        public override double DistanceTo(Point pos)
        {
            return Dist(pos) * Cable.DistanceFactor;
        }

        public override bool IncludesPos(Point pos)
        {
            return Dist(pos) <= Cable.SegmentWidth && cableSegment.Parent.IsCompleted;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return RectBounds.IntersectsWith(rect);
        }
    }
}
