using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableJoint : IClickable, IMovable
    {
        public CableJoint()
        {
            hitbox = new CircleHitbox(this, new Point(), 2, 0.1);
        }

        public CableSegment Before, After;

        CircleHitbox hitbox;
        Cable cable;
        Point position;
        bool isSelected;

        public CableJointRenderer Renderer
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
                hitbox = value as CircleHitbox;
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

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Before?.Renderer.OnPositionChanged();
                After?.Renderer.OnPositionChanged();
                Renderer.OnPositionChanged();
            }
        }

        public void UpdateHitbox()
        {
            (Hitbox as CircleHitbox).Center = position;
        }

        public void Move(Vector vector)
        {
            Position += vector;
            UpdateHitbox();
        }
    }
}
