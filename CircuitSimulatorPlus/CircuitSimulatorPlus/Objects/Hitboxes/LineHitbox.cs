using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class LineHitbox : Hitbox
    {
        protected double width;
        protected double x, y, z;
        protected bool vert;

        public LineHitbox(double width, double x, double y, double z, bool vert)
        {
            this.width = width;
            this.x = x;
            this.y = y;
            this.z = z;
            this.vert = vert;
        }

        protected LineHitbox()
        {
        }

        public override Rect RectBounds
        {
            get
            {
                //Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
                //Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
                //Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

                //if ((cableSegment.Index & 1) != 0)
                //    return new Rect(point.X, Math.Min(lastPoint.Y, nextPoint.Y), 0, Math.Abs(nextPoint.Y - lastPoint.Y));
                //return new Rect(Math.Min(lastPoint.X, nextPoint.X), point.Y, Math.Abs(nextPoint.X - lastPoint.X), 0);

                if (vert)
                {
                    return new Rect(y, Math.Min(x, z), 0, Math.Abs(x - z));
                }
                return new Rect(Math.Min(x, z), y, Math.Abs(x - z), 0);
            }
        }

        double Dist(Point pos)
        {
            //Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
            //Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
            //Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

            //double startX = vert ? point.X : lastPoint.X;
            //double startY = vert ? lastPoint.Y : point.Y;
            //double endX = vert ? point.X : nextPoint.X;
            //double endY = vert ? nextPoint.Y : point.Y;

            //double dist = vert ? Math.Abs(pos.X - startX) : Math.Abs(pos.Y - startY);

            //double lastDist = vert ? Math.Abs(pos.Y - startY) : Math.Abs(pos.X - startX);
            //double nextDist = vert ? Math.Abs(pos.Y - endY) : Math.Abs(pos.X - endX);

            //double len = vert ? Math.Abs(endY - startY) : Math.Abs(endX - startX);

            //if (lastDist < len && nextDist < len)
            //    return dist - 1e-7;

            //double minDist = Math.Min(lastDist, nextDist);
            //double sideDist = Math.Max(dist, minDist);

            //if (dist > minDist)
            //    return sideDist - 1e-7;

            //return sideDist;

            //double startX = vert ? y : x;
            //double startY = vert ? x : y;
            //double endX = vert ? y : z;
            //double endY = vert ? z : y;

            double dist = vert ? Math.Abs(pos.X - y) : Math.Abs(pos.Y - y);

            double lastDist = vert ? Math.Abs(pos.Y - x) : Math.Abs(pos.X - x);
            double nextDist = vert ? Math.Abs(pos.Y - z) : Math.Abs(pos.X - z);

            double len = Math.Abs(z - x);

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
            return Dist(pos) <= Cable.SegmentWidth;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return RectBounds.IntersectsWith(rect);
        }
    }
}
