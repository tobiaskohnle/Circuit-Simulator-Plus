using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CircleHitbox : Hitbox
    {
        public Point Center
        {
            get; set;
        }

        public override Rect RectBounds
        {
            get
            {
                return new Rect(Center.X - radius, Center.Y - radius, 2 * radius, 2 * radius);
            }
        }

        double radius;

        public CircleHitbox(Point center, double radius, double distanceFactor) : base(distanceFactor)
        {
            Center = center;
            this.radius = radius;
        }

        public override double DistanceTo(Point pos)
        {
            return distanceFactor * (pos - Center).LengthSquared;
        }

        public override bool IncludesPos(Point pos)
        {
            return (pos - Center).LengthSquared <= radius * radius;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            double halfWidth = rect.Width / 2;
            double halfHeight = rect.Height / 2;
            double distCenterX = Math.Abs(Center.X - rect.X - halfWidth);
            double distCenterY = Math.Abs(Center.Y - rect.Y - halfHeight);

            if (halfWidth + radius < distCenterX)
                return false;
            if (halfHeight + radius < distCenterY)
                return false;
            if (halfWidth >= distCenterX)
                return true;
            if (halfHeight >= distCenterY)
                return true;
            return (distCenterX - halfWidth) * (distCenterX - halfWidth)
                + (distCenterY - halfHeight) * (distCenterY - halfHeight)
                < radius * radius;
        }
    }
}
