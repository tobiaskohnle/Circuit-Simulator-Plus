using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class ExtendedCircleHitbox : Hitbox
    {
        public Point PointA
        {
            get; set;
        }
        public Point PointB
        {
            get; set;
        }

        public override Rect RectBounds
        {
            get
            {
                return new Rect(PointA.X - radius, PointA.Y - radius, PointB.X + radius, PointB.Y + radius);
            }
        }

        double radius;

        public ExtendedCircleHitbox(Point a, Point b, double radius)
        {
            PointA = a;
            PointB = b;
            this.radius = radius;
        }

        public override double DistanceTo(Point pos)
        {
            return ConnectionNode.DistanceFactor * (pos - Center).LengthSquared;
        }

        public override bool IncludesPos(Point pos)
        {
            return (pos - Center).LengthSquared <= radius * radius;
        }

        double GetDist(Point pos)
        {
            double a = (pos - PointA).Length;
            double b = (pos - PointB).Length;
            double c = (PointA - PointB).Length;

            if (a >= c || b >= c)
            {
                return Math.Min(a, b);
            }

            double s = a / 2 + b / 2 + c / 2;
            double area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

            return area / c;
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
