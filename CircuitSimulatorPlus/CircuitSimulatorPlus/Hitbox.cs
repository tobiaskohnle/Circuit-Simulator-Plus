using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Hitbox
    {
        protected Rect bounds;
        protected object attachedObject;
        protected double distanceFactor;

        public Hitbox(Rect bounds)
        {
            this.bounds = bounds;
        }

        public bool IncludesPos(Point pos)
        {
            return pos.X >= bounds.X
                && pos.Y >= bounds.Y
                && pos.X <= bounds.X + bounds.Width
                && pos.Y <= bounds.Y + bounds.Height;
        }

        public Point Center
        {
            get {
                return new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            }
        }

        public double DistanceSquaredTo(Point pos)
        {
            return (pos - Center).LengthSquared;
        }
    }
}
