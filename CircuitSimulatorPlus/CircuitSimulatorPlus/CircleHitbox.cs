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

        public override bool IncludedIn(Rect rect)
        {
            double distCenterX = Math.Abs(Center.X - rect.X - rect.Width / 2);
            double distCenterY = Math.Abs(Center.Y - rect.Y - rect.Height / 2);

            if (rect.Width / 2 + radius > distCenterX)
                return false;
            if (rect.Height / 2 + radius > distCenterY)
                return false;
            if (rect.Width / 2 <= distCenterX)
                return true;
            if (rect.Height / 2 <= distCenterY)
                return true;
            return (Center - new Point(rect.Width / 2, rect.Height / 2)).LengthSquared < radius * radius;
        }
    }
}
