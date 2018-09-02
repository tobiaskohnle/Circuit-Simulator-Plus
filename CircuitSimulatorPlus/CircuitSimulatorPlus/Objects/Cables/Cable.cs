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
            
            new CableSegment(this, amtSegments++);
            new CableSegment(this, amtSegments++);
        }

        public const double DistanceFactor = 0.2;
        public const double SegmentWidth = 1.0;

        public bool IsCompleted;

        int amtSegments;

        bool vertical;

        public event Action OnLayoutChanged;
        public event Action OnPointsChanged;

        List<Point> points = new List<Point>();

        public void MovePoint(int index, Vector vector)
        {
            if (index > 0 && index < points.Count - 1)
                points[index] += vector;
        }

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
            points.Add(point);
            vertical = !vertical;

            new CableSegment(this, amtSegments++);

            OnPointsChanged?.Invoke();
        }
    }
}
