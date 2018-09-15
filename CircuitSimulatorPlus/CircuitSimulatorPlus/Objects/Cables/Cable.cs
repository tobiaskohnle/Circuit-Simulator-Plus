using System;
using System.Collections.Generic;
using System.Linq;
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

        public Cable(ConnectionNode startNode, ConnectionNode endNode) : this(startNode)
        {
            ConnectTo(endNode);
        }

        public const double DistanceFactor = 1.5;
        public const double HitboxWidth = 1.0;

        public List<CableSegment> Segments = new List<CableSegment>();

        public void SplitSegment(int index)
        {
            if (index > 0 && index < Points.Count + 1)
            {
                if ((index & 1) != 0)
                {
                    double centerY = GetPoint(index - 1).Y / 2 + GetPoint(index + 1).Y / 2;
                    double centerX = GetPoint(index).X;
                    Points[index - 1] = new Point(centerX, centerY);

                    AddSegment(index + 1, new Point(centerX + 0.5, 0));
                    AddSegment(index + 0, new Point(centerX - 0.5, 0));
                }
                else
                {
                    double centerX = GetPoint(index - 1).X / 2 + GetPoint(index + 1).X / 2;
                    double centerY = GetPoint(index).Y;
                    Points[index - 1] = new Point(centerX, centerY);

                    AddSegment(index + 1, new Point(0, centerY + 0.5));
                    AddSegment(index + 0, new Point(0, centerY - 0.5));
                }
            }
        }

        public bool IsCompleted;

        public event Action OnPointsChanged;

        public List<Point> Points = new List<Point>();

        public void MovePoint(int index, Vector vector)
        {
            if (index > 0 && index < Points.Count + 1)
                Points[index - 1] += vector;

            UpdateHitbox();
            OnPointsChanged?.Invoke();
        }

        public Point GetPoint(int index)
        {
            if (index <= 0)
                return StartPos;
            if (index >= Points.Count + 1)
                return EndPos;
            return Points[index - 1];
        }

        public Point StartPos
        {
            get
            {
                return StartNode.CableAnchorPoint;
            }
        }
        public Point EndPos
        {
            get
            {
                if (IsCompleted)
                    return EndNode.CableAnchorPoint;
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

                if (value is InputNode)
                {
                    inputNode = value as InputNode;
                }
                if (value is OutputNode)
                {
                    outputNode = value as OutputNode;
                    outputNode.OnStateChanged += RaiseStateChanged;
                    OnStateChanged?.Invoke();
                }

                UpdateHitbox();
                OnPointsChanged?.Invoke();
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

                if (value is InputNode)
                {
                    inputNode = value as InputNode;
                }
                if (value is OutputNode)
                {
                    outputNode = value as OutputNode;
                    outputNode.OnStateChanged += RaiseStateChanged;
                    OnStateChanged?.Invoke();
                }

                UpdateHitbox();
                OnPointsChanged?.Invoke();
            }
        }

        OutputNode outputNode;
        InputNode inputNode;

        public event Action OnStateChanged;
        public void RaiseStateChanged()
        {
            OnStateChanged?.Invoke();
        }

        public bool State
        {
            get
            {
                return outputNode?.State ?? false;
            }
        }

        public void UpdateHitbox()
        {
            foreach (CableSegment segment in Segments)
            {
                segment.UpdateHitbox();
            }
        }

        public void AutoComplete()
        {
            bool endsWithInputNode = endNode is InputNode;

            Point lastPoint = Points.Count > 0 ? Points[Points.Count - 1] : StartPos;

            bool leftToRight = !endsWithInputNode && endNode.CableAnchorPoint.X > lastPoint.X;
            bool rightToLeft = endsWithInputNode && endNode.CableAnchorPoint.X < lastPoint.X;

            Point referencePoint = Points.Count > 1 ? Points[Points.Count - 2] : StartPos;

            if (rightToLeft || leftToRight)
            {
                if ((Points.Count & 1) == 0)
                {
                    AddSegment(referencePoint);
                }

                AddSegment(referencePoint + (EndPos - referencePoint) / 2);
                AddSegment(endNode.CableAnchorPoint);
            }
            else if ((Points.Count & 1) == 0)
            {
                AddSegment(lastPoint + (endNode.CableAnchorPoint - lastPoint) / 2);
            }
        }
        public void ConnectTo(ConnectionNode endNode, bool autoComplete = true)
        {
            EndNode = endNode;
            if (autoComplete)
            {
                AutoComplete();
            }

            if (startNode is InputNode)
            {
                (startNode as InputNode).ConnectedCable = this;
                (startNode as InputNode).OnPositionChanged += OnPointsChanged;
            }
            if (startNode is OutputNode)
            {
                (startNode as OutputNode).ConnectedCables.Add(this);
                (startNode as OutputNode).OnPositionChanged += OnPointsChanged;
            }
            if (endNode is InputNode)
            {
                (endNode as InputNode).ConnectedCable = this;
                (endNode as InputNode).OnPositionChanged += OnPointsChanged;
            }
            if (endNode is OutputNode)
            {
                (endNode as OutputNode).ConnectedCables.Add(this);
                (endNode as OutputNode).OnPositionChanged += OnPointsChanged;
            }

            IsCompleted = true;

            UpdateHitbox();
            OnPointsChanged?.Invoke();
        }

        public void RemoveSegment(int index)
        {
            if (index > 0 && index <= Points.Count)
            {
                Points.RemoveAt(index - 1);

                Segments.RemoveAt(index);

                for (int i = index; i < Segments.Count; i++)
                    Segments[i].Index--;

                UpdateHitbox();
                OnPointsChanged?.Invoke();
            }
        }
        public void AddSegment(int index, Point point)
        {
            Points.Insert(index - 1, MainWindow.Self.Round(point, 0.5));

            Segments.Insert(index, new CableSegment(this, index));

            for (int i = index + 1; i < Segments.Count; i++)
                Segments[i].Index++;

            UpdateHitbox();
            OnPointsChanged?.Invoke();
        }
        public void AddSegment(Point point)
        {
            Points.Add(MainWindow.Self.Round(point, 0.5));

            Segments.Add(new CableSegment(this, Segments.Count));

            UpdateHitbox();
            OnPointsChanged?.Invoke();
        }

        public void Remove()
        {
            foreach (CableSegment segment in Segments.ToList())
            {
                segment.Remove();
            }
            MainWindow.Self.Cables.Remove(this);
        }

        public void Clear()
        {
            inputNode?.Clear();
        }
    }
}
