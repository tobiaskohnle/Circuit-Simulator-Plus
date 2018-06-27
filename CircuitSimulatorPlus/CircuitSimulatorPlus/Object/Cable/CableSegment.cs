using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableSegment : IClickable, IMovable
    {
        public CableSegment()
        {
            hitbox = new LineHitbox(this, new Point(), new Point(), Cable.CableWidth, 1);
        }

        LineHitbox hitbox;
        Cable cable;
        CableJoint a, b;
        bool isSelected;

        public CableSegmentRenderer Renderer
        {
            get; set;
        }

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

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                Renderer.OnSelectionChanged();
            }
        }

        public void UpdateHitbox()
        {
            (Hitbox as LineHitbox).A = a.Position;
            (Hitbox as LineHitbox).B = b.Position;
        }

        public void Move(Vector vector)
        {
            a.Move(vector);
            b.Move(vector);
        }

        public CableJoint A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
            }
        }
        public CableJoint B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
            }
        }

        public bool State
        {
            get; set;
        }
    }
}
