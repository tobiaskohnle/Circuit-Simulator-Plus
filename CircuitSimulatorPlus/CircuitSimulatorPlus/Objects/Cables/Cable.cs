﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Cable : IClickable, IMovable
    {
        public Cable(ConnectionNode startNode)
        {
            StartNode = startNode;

            UpdatePoints();

            hitbox = new CableHitbox(Points);

            new CableRenderer(this);
            IsRendered = true;

            MainWindow.Self.OnLastCanvasPosChanged += UpdatePoints;
            startNode.OnPositionChanged += UpdatePoints;
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

        List<double> segmentPoints = new List<double>();
        public List<double> SegmentPoints
        {
            get
            {
                return segmentPoints;
            }
            set
            {
                segmentPoints = value;
                hitbox.Points = value;
                UpdatePoints();
                OnPointsChanged?.Invoke();
            }
        }

        public List<double> Points = new List<double>();

        public void UpdatePoints()
        {
            while (Points.Count < segmentPoints.Count + 4)
            {
                Points.Add(0);
            }
            while (Points.Count > segmentPoints.Count + 4)
            {
                Points.RemoveAt(Points.Count - 1);
            }

            Points[0] = StartPos.X;
            Points[1] = StartPos.Y;
            
            for (int i = 0; i < segmentPoints.Count; i++)
                Points[i + 2] = segmentPoints[i];

            Points[Points.Count - 2] = vertical ? EndPos.Y : EndPos.X;
            Points[Points.Count - 1] = vertical ? EndPos.X : EndPos.Y;

            Console.WriteLine(Points.Count);
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
            throw new NotImplementedException();
        }

        public void ConnectTo(ConnectionNode endNode)
        {
            EndNode = endNode;
            IsCompleted = true;
        }

        public void AddPoint(Point point)
        {
            if (vertical)
                SegmentPoints.Add(point.Y);
            else
                SegmentPoints.Add(point.X);
            vertical = !vertical;

            UpdatePoints();
            OnPointsChanged?.Invoke();
        }

        public void MoveSegment(int index, Vector vector)
        {

        }

        public void Move(Vector vector)
        {
            if (hitbox.SegmentIndex > 0 && hitbox.SegmentIndex < segmentPoints.Count)
            {
                bool vert = (hitbox.SegmentIndex & 1) != 0;
                segmentPoints[hitbox.SegmentIndex - 1] += vert ? vector.X : vector.Y;
                UpdatePoints();
            }
        }
    }
}
