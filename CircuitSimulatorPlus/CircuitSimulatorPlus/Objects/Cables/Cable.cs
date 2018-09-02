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

        public event Action OnLayoutChanged;
        public event Action OnPointsChanged;

        List<double> points = new List<double>();
        public List<double> SegmentPoints
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                hitbox.Points = value;
                OnPointsChanged?.Invoke();
            }
        }

        public double GetPointAt(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();
            if (index == 0)
                return StartPos.X;
            if (index == 1)
                return StartPos.Y;

            if (index == 2 + points.Count)
                return vertical ? EndPos.Y : EndPos.X;
            if (index == 2 + points.Count + 1)
                return vertical ? EndPos.X : EndPos.Y;
            if (index > 2 + points.Count + 1)
                throw new ArgumentOutOfRangeException();

            return points[index - 2];
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

        ConnectionNode startNode;
        public ConnectionNode StartNode
        {
            get
            {
                return startNode;
            }
            set
            {
                startNode = value;
                if (value is OutputNode)
                    outputNode = value as OutputNode;
            }
        }

        ConnectionNode endNode;
        public ConnectionNode EndNode
        {
            get
            {
                return endNode;
            }
            set
            {
                endNode = value;
                if (value is OutputNode)
                    outputNode = value as OutputNode;
            }
        }

        public CableHitbox hitbox; // FIXME
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

        OutputNode outputNode;

        public bool State
        {
            get
            {
                return outputNode?.State ?? false;
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
                SegmentPoints.Add(point.Y);
            else
                SegmentPoints.Add(point.X);
            vertical = !vertical;

            OnPointsChanged?.Invoke();
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
