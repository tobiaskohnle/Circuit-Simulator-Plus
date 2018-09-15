using System;
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

            hitbox = new LineSegmentHitbox(this);

            Add();
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

        public double SnapSize
        {
            get
            {
                return 0.5;
            }
        }

        public Point Position
        {
            get
            {
                return Parent.GetPoint(Index);
            }
        }

        public void Move(Vector vector)
        {
            Parent.MovePoint(Index, vector);
            OnPointsChanged?.Invoke();
        }

        public void SplitSegment()
        {
            Parent.SplitSegment(Index);
        }

        public void Add()
        {
            hitbox = new LineSegmentHitbox(this);
            MainWindow.Self.ClickableObjects.Add(this);
            MainWindow.Self.refs.Add(new WeakReference<IClickable>(this));//temp
            new CableSegmentRenderer(this);
            IsRendered = true;
        }
        public void Remove()
        {
            Parent.RemoveSegment(Index);
            IsRendered = false;
            MainWindow.Self.ClickableObjects.Remove(this);
        }
    }
}
