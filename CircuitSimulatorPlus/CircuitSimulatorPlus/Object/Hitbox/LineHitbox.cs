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
        public Point A, B;
        double width;

        public LineHitbox(object attachedObject, Point a, Point b, double width, double distanceFactor) : base(attachedObject, distanceFactor)
        {
            this.A = a;
            this.B = b;
            this.width = width;
        }

        private Point M(Point d)
        {
            return A + ((A - B).Length + (A - d).Length - (B - d).Length) / 2 * (B - A) / (A - B).Length;
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
