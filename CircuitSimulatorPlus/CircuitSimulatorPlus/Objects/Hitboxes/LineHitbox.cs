﻿using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class LineHitbox : Hitbox
    {
        protected double width;
        protected double x, y, z;
        protected bool vert;

        protected LineHitbox(double width)
        {
            this.width = width;
        }

        public abstract void UpdateHitbox();

        public override Rect RectBounds
        {
            get
            {
                if (vert)
                {
                    return new Rect(y, Math.Min(x, z), 0, Math.Abs(x - z));
                }
                return new Rect(Math.Min(x, z), y, Math.Abs(x - z), 0);
            }
        }

        double Dist(Point pos)
        {
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
            return Dist(pos) <= Cable.HitboxWidth;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return RectBounds.IntersectsWith(rect);
        }
    }
}
