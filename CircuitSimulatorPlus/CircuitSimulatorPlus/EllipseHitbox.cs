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
    }
}
