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
        public Point Center { get; set; }
        double radius;

        public CircleHitbox(object attachedObject, Point center, double radius, double distanceFactor)
            : base(attachedObject, distanceFactor)
        {
            Center = center;
            this.radius = radius;
        }

        double Dist(Vector vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y;
        }

        public override double DistanceTo(Point pos)
        {
            return distanceFactor * Dist(pos - Center);
        }

        public override bool IncludesPos(Point pos)
        {
            return Dist(pos - Center) <= radius * radius;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            double distCenterX = Math.Abs(Center.X - rect.X - rect.Width / 2);
            double distCenterY = Math.Abs(Center.Y - rect.Y - rect.Height / 2);
            double halfWidth = rect.Width / 2;
            double halfHeight = rect.Height / 2;

            if (halfWidth + radius > distCenterX)
                return false;
            if (halfHeight + radius > distCenterY)
                return false;
            if (halfWidth <= distCenterX)
                return true;
            if (halfHeight <= distCenterY)
                return true;
            return (Center.X - halfWidth) * (Center.X - halfWidth)
                + (Center.Y - halfHeight) * (Center.Y - halfHeight)
                < radius * radius;
        }
    }
}
