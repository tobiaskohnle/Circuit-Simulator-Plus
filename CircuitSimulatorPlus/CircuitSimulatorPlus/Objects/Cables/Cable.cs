using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Cable
    {
        public Cable(ConnectionNode startNode)
        {
            StartNode = startNode;

            new CableRenderer(this);
            IsRendered = true;
        }

        public const double DistanceFactor = 0.2;
        public const double SegmentWidth = 1.0;

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

        List<Point> points = new List<Point>();
        public List<Point> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                OnPointsChanged?.Invoke();
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
            throw new NotImplementedException();
        }

        public void ConnectTo(ConnectionNode endNode)
        {
            EndNode = endNode;
            IsCompleted = true;
        }

        public void AddPoint(Point point)
        {
            Points.Add(point);
            vertical = !vertical;

            OnPointsChanged?.Invoke();
        }
    }
}
