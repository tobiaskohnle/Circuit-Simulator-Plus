using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class CableJoint : IClickable
    {
        CircleHitbox hitbox;
        bool isSelected;

        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
            }
            set
            {
                hitbox = value as CircleHitbox;
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
    }
}
