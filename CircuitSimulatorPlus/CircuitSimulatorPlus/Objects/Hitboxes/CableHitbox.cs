using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableHitbox : Hitbox
    {
        public CableHitbox(double distanceFactor) : base(distanceFactor)
        {
        }

        public override Rect RectBounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double DistanceTo(Point pos)
        {
            throw new NotImplementedException();
        }

        public override bool IncludesPos(Point pos)
        {
            throw new NotImplementedException();
        }

        public override bool IsIncludedIn(Rect rect)
        {
            throw new NotImplementedException();
        }
    }
}
