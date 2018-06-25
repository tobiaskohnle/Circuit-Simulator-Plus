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
        Point a, b;
        double width;

        public LineHitbox(object attachedObject, Point a, Point b, double width, double distanceFactor) : base(attachedObject, distanceFactor)
        {
            this.a = a;
            this.b = b;
            this.width = width;
        }

        private Point M(Point d)
        {
            return a + (a - b) * ((a - d).Length + (b - d).Length) / 2;
        }

        public override double DistanceTo(Point pos)
        {
            return (M(pos) - pos).LengthSquared;
        }

        public override bool IncludesPos(Point pos)
        {
            return (M(pos) - pos).LengthSquared < width * width;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return false;
        }
    }
}
