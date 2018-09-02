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
        public List<Point> Points;
        public int Index;

        public CableSegment(List<Point> points, int index)
        {
            Points = points;
            Index = index;
        }

        public event Action OnRenderedChanged;
        bool isRendered;
        public bool IsRendered
        {
            get
            {
                return isRendered;
            }
            set
            {
                isRendered = value;
                OnRenderedChanged?.Invoke();
            }
        }

        LineHitbox hitbox;
        public Hitbox Hitbox
        {
            get
            {
                return hitbox as Hitbox;
            }
            set
            {
                hitbox = value as LineHitbox;
            }
        }

        public event Action OnSelectedChanged;
        bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnSelectedChanged?.Invoke();
            }
        }

        public void Move(Vector vector)
        {
            if (Index > 0 && Index < Points.Count - 1)
                Points[Index] += vector;
        }
    }
}
