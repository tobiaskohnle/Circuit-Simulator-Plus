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
        Cable parent;
        int index;

        double distance;

        public LineHitbox(Cable parent, int index) : base(DistanceFactor)
        {
            this.parent = parent;
            this.index = index;
        }

        public const double DistanceFactor = 0.2;
        public const double Width = 1.0;

        public override Rect RectBounds
        {
            get
            {
                return parent.LineBounds(index);
            }
        }

        public override double DistanceTo(Point pos)
        {
            return parent.SegmentDistance(index, pos) * DistanceFactor;
        }

        public override bool IncludesPos(Point pos)
        {
            return parent.SegmentSelected(index, pos) && parent.SegmentDistance(index, pos) < Width;
        }

        public override bool IsIncludedIn(Rect rect)
        {
            return parent.LineBounds(index).IntersectsWith(rect);
        }
    }
}
