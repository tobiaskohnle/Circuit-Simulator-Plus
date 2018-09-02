using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Cable : IClickable, IMovable
    {
        public Cable(ConnectionNode startNode)
        {
            StartNode = startNode;

            hitbox = new CableHitbox(points);

            new CableRenderer(this);
            IsRendered = true;
        }

        public event Action OnRenderedChanged;
        protected bool isRendered;
        public bool IsRendered
        {
            get
            {
                return isRendered;
            }
            set
            {
                if (isRendered != value)
                {
                    isRendered = value;
                    OnRenderedChanged?.Invoke();
                }
            }
        }

        public bool IsCompleted;

        bool vertical;

        List<double> points;
        public List<double> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                hitbox.Points = value;
            }
        }

        public Point StartPos
        {
            get
            {
                return StartNode.Position;
            }
        }
        public Point EndPos
        {
            get
            {
                if (IsCompleted)
                    return EndNode.Position;
                return MainWindow.Self.LastCanvasPos;
            }
        }

        public ConnectionNode StartNode
        {
            get; set;
        }

        public ConnectionNode EndNode
        {
            get; set;
        }

        CableHitbox hitbox;
        public Hitbox Hitbox
        {
            get
            {
                return hitbox as Hitbox;
            }
            set
            {
                hitbox = value as CableHitbox;
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

        /// <summary>
        /// Adds points to make a valid cable
        /// </summary>
        public void AutoComplete()
        {

        }

        public void AddPoint(Point point)
        {
            if (vertical)
                Points.Add(point.X);
            else
                Points.Add(point.Y);
            vertical = !vertical;
        }

        public void MoveSegment(int index, Vector vector)
        {

        }

        public void Move(Vector vector)
        {
            throw new NotImplementedException();
        }
    }
}
