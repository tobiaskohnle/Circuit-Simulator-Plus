using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class EllipseHitbox : Hitbox
    {
        public EllipseHitbox(Rect bounds) : base(bounds)
        {
        }

        public new bool IncludesPos(Point pos)
        {
            Vector vector = pos - Center;
            return vector.X * vector.X / bounds.Width / bounds.Width
                + vector.Y * vector.Y / bounds.Height / bounds.Height <= 1;
        }
    }
}
