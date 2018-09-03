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

            Segments.Add(new CableSegment(this, Segments.Count));
            Segments.Add(new CableSegment(this, Segments.Count));
        }

        public const double DistanceFactor = 0.9;
        public const double SegmentWidth = 1.0;

        public List<CableSegment> Segments = new List<CableSegment>();

        public void SplitSegment(int index)
        {
            if (index > 0 && index < points.Count + 1)
            {
                if ((index & 1) != 0)
                {
                    double centerY = GetPoint(index - 1).Y / 2 + GetPoint(index + 1).Y / 2;
                    double centerX = GetPoint(index).X;
                    points[index - 1] = new Point(centerX, centerY);

                    AddPoint(index + 1, new Point(centerX - 0.5, 0));
                    AddPoint(index + 0, new Point(centerX + 0.5, 0));
                }
                else
                {
                    double centerX = GetPoint(index - 1).X / 2 + GetPoint(index + 1).X / 2;
                    double centerY = GetPoint(index).Y;
                    points[index - 1] = new Point(centerX, centerY);

                    AddPoint(index + 1, new Point(0, centerY - 0.5));
                    AddPoint(index + 0, new Point(0, centerY + 0.5));
                }
            }
        }

        public bool IsCompleted;

        bool vertical;

        public event Action OnLayoutChanged;
        public event Action OnPointsChanged;

        List<Point> points = new List<Point>();

        public void MovePoint(int index, Vector vector)
        {
            if (index > 0 && index < points.Count + 1)
                points[index - 1] += vector;
        }

        public Point GetPoint(int index)
        {
            if (index <= 0)
                return StartPos;
            if (index >= points.Count + 1)
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

        public void AddPoint(int index, Point point)
        {
            points.Insert(index - 1, point);
            vertical = !vertical;

            Segments.Insert(index, new CableSegment(this, index));

            for (int i = index + 1; i < Segments.Count; i++)
                Segments[i].Index++;

            OnPointsChanged?.Invoke();
        }

        public void AddPoint(Point point)
        {
            points.Add(point);
            vertical = !vertical;

            Segments.Add(new CableSegment(this, Segments.Count));

            OnPointsChanged?.Invoke();
        }
    }
}
