using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableHitbox : Hitbox
    {
        public List<double> Points;

        Point lastPosScanned;

        public int SegmentIndex;
        public double SegmentDistance;

        public CableHitbox(List<double> points) : base(DistanceFactor)
        {
            Points = points;
        }

        public const double DistanceFactor = 0.2;
        public const double SegmentWidth = 1.0;

        public override Rect RectBounds
        {
            get
            {
                double minX = Double.PositiveInfinity;
                double maxX = Double.NegativeInfinity;
                double minY = Double.PositiveInfinity;
                double maxY = Double.NegativeInfinity;

                for (int i = 0; i < Points.Count; i++)
                {
                    if ((i & 1) == 0)
                    {
                        if (Points[i] < minX)
                            minX = Points[i];
                        if (Points[i] > maxX)
                            maxX = Points[i];
                    }
                    else
                    {
                        if (Points[i] < minY)
                            minY = Points[i];
                        if (Points[i] > maxY)
                            maxY = Points[i];
                    }
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public override double DistanceTo(Point pos)
        {
            HitScan(pos);
            return SegmentDistance * distanceFactor;
        }

        public override bool IncludesPos(Point pos)
        {
            HitScan(pos);
            return SegmentDistance < SegmentWidth;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            for (int i = 2; i < Points.Count - 4; i++)
            {
                bool vertical = (i & 1) != 0;

                double lastX = vertical ? Points[i] : Points[i - 1];
                double lastY = vertical ? Points[i - 1] : Points[i];
                double nextX = vertical ? Points[i] : Points[i + 1];
                double nextY = vertical ? Points[i + 1] : Points[i];

                if (Math.Min(lastX, nextX) >= rect.X - Math.Abs(nextX - lastX)
                    && Math.Min(lastX, nextX) <= rect.X + rect.Width
                    && Math.Min(lastY, nextY) >= rect.Y - Math.Abs(nextY - lastY)
                    && Math.Min(lastY, nextY) <= rect.Y + rect.Height) return true;
            }

            return false;
        }

        public void HitScan(Point pos)
        {
            if (pos != lastPosScanned)
            {
                lastPosScanned = pos;

                SegmentIndex = -1;
                SegmentDistance = Double.PositiveInfinity;

                bool prioritizedFound = false;

                for (int i = 2; i < Points.Count - 2; i++)
                {
                    double point = Points[i - 1];
                    double lastPoint = Points[i - 1 - 1];
                    double nextPoint = Points[i - 1 + 1];

                    bool vert = (i & 1) != 0;

                    double startX = vert ? point : lastPoint;
                    double startY = vert ? lastPoint : point;
                    double endX = vert ? point : nextPoint;
                    double endY = vert ? nextPoint : point;

                    double dist = vert ? Math.Abs(pos.X - startX) : Math.Abs(pos.Y - startY);

                    double lastDist = vert ? Math.Abs(pos.Y - startY) : Math.Abs(pos.X - startX);
                    double nextDist = vert ? Math.Abs(pos.Y - endY) : Math.Abs(pos.X - endX);

                    double len = vert ? Math.Abs(endY - startY) : Math.Abs(endX - startX);

                    if (lastDist < len && nextDist < len)
                    {
                        if (dist < SegmentDistance || dist == SegmentDistance && !prioritizedFound)
                        {
                            SegmentDistance = dist;
                            prioritizedFound = true;
                            SegmentIndex = i - 2;
                        }
                        continue;
                    }

                    double mindist = Math.Min(lastDist, nextDist);
                    double sidedist = Math.Max(dist, mindist);

                    bool prioritize = dist > mindist;

                    if (sidedist < SegmentDistance || sidedist == SegmentDistance && prioritize && !prioritizedFound)
                    {
                        SegmentDistance = sidedist;
                        prioritizedFound = prioritize;
                        SegmentIndex = i - 2;
                    }
                }
            }
        }
    }
}
