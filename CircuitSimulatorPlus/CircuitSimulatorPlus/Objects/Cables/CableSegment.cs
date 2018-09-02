using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CableSegment : IClickable, IMovable
    {
        public Cable Parent;

        public int Index;

        public CableSegment(Cable parent, int index)
        {
            Parent = parent;
            Index = index;

            new CableSegmentRenderer(this);
            IsRendered = true;

            hitbox = new LineHitbox(parent.Points, index);

            MainWindow.Self.ClickableObjects.Add(this);
        }

        public event Action OnPointsChanged;

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
            if (Index > 0 && Index < Parent.Points.Count - 1)
                Parent.Points[Index] += vector;
            OnPointsChanged?.Invoke();
        }
    }
}
