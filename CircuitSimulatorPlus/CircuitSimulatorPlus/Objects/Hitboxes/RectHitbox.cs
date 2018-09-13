using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class RectHitbox : Hitbox
    {
        public Rect Bounds;

        public RectHitbox(Rect bounds)
        {
            Bounds = bounds;
        }

        public override Rect RectBounds
        {
            get
            {
                return Bounds;
            }
        }

        public override double DistanceTo(Point pos)
        {
            return Gate.DistanceFactor;
        }

        public override bool IncludesPos(Point pos)
        {
            return pos.X >= Bounds.X
                && pos.Y >= Bounds.Y
                && pos.X <= Bounds.X + Bounds.Width
                && pos.Y <= Bounds.Y + Bounds.Height;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return Bounds.X >= rect.X - Bounds.Width
                && Bounds.X <= rect.X + rect.Width
                && Bounds.Y >= rect.Y - Bounds.Height
                && Bounds.Y <= rect.Y + rect.Height;
        }
    }
}
