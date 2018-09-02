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

            points.Add(StartPos);
            points.Add(EndPos);
            
            Segments.Add(new CableSegment(this, Segments.Count));
            Segments.Add(new CableSegment(this, Segments.Count));
        }

        public const double DistanceFactor = 0.2;
        public const double SegmentWidth = 1.0;

        public List<CableSegment> Segments = new List<CableSegment>();

        public bool IsCompleted;

        bool vertical;

        public event Action OnLayoutChanged;
        public event Action OnPointsChanged;

        List<Point> points = new List<Point>();

        public Point GetPoint(int index)
        {
            if (index <= 0)
                return StartPos;
            if (index >= points.Count - 1)
                return EndPos;
            return points[index - 1];
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

        OutputNode outputNode;

        public bool State
        {
            get
            {
                return outputNode?.State ?? false;
            }
        }

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
            Points.Insert(Points.Count - 1, point);
            vertical = !vertical;

            Segments.Add(new CableSegment(this, Segments.Count));

            OnPointsChanged?.Invoke();
        }
    }
}
