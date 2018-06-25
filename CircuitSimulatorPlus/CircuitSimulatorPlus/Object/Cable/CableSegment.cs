using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableSegment : IClickable
    {
        LineHitbox hitbox;
        CableJoint a, b;
        bool isSelected;

        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
            }
            set
            {
                hitbox = value as LineHitbox;
            }
        }

        public bool IsMovable
        {
            get
            {
                return true;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }

        public void UpdateHitbox()
        {
            throw new NotImplementedException();
        }

        public void Move(Vector vector)
        {
            a.Move(vector);
            b.Move(vector);
        }
    }
}
