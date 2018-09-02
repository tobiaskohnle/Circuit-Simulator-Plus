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
        List<Point> points;
        int index;

        public CableSegment(List<Point> points, int index)
        {
            this.points = points;
            this.index = index;
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
            if (index > 0 && index < points.Count - 1)
                points[index] += vector;
        }
    }
}
