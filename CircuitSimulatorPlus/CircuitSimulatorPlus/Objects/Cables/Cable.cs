using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Cable : IClickable, IMovable
    {
        public Cable()
        {
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

        public bool StartAtOutputNode;

        bool vertical;

        public List<double> Points;

        public Point StartPos
        {
            get
            {
                return StartAtOutputNode ? OutputNode.Position : InputNode.Position;
            }
        }
        public Point EndPos
        {
            get
            {
                if (IsCompleted)
                    return StartAtOutputNode ? InputNode.Position : OutputNode.Position;
                return MainWindow.Self.LastCanvasPos;
            }
        }

        public OutputNode OutputNode
        {
            get; set;
        }

        public InputNode InputNode
        {
            get; set;
        }

        public Hitbox Hitbox
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSelected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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

        public void Move(Vector vector)
        {
            throw new NotImplementedException();
        }

        Point lastPosScanned;

        int segmentIndex;
        double segmentDistance;

        public void HitScan(Point pos)
        {
            if (pos != lastPosScanned)
            {
                lastPosScanned = pos;

                segmentIndex = -1;
                segmentDistance = Double.PositiveInfinity;

                bool prioritizedFound = false;

                for (int i = 0; i < Points.Count - 4; i++)
                {
                    var point = Points[i + 2];
                    var lastPoint = Points[i + 2 - 1];
                    var nextPoint = Points[i + 2 + 1];

                    var vert = (i & 1) != 0;

                    double startX = vert ? point : lastPoint;
                    double startY = vert ? lastPoint : point;
                    double endX = vert ? point : nextPoint;
                    double endY = vert ? nextPoint : point;

                    double dist = vert ? Math.Abs(pos.X - startX) : Math.Abs(pos.Y - startY);

                    double lastDist = vert ? Math.Abs(pos.Y - startY) : Math.Abs(pos.X - startX);
                    double nextDist = vert ? Math.Abs(pos.Y - endY) : Math.Abs(pos.X - endX);

                    var len = vert ? Math.Abs(endY - startY) : Math.Abs(endX - startX);

                    if (lastDist < len && nextDist < len)
                    {
                        if (dist < segmentDistance || dist == segmentDistance && !prioritizedFound)
                        {
                            segmentDistance = dist;
                            prioritizedFound = true;
                            segmentIndex = i;
                        }
                        continue;
                    }

                    double mindist = Math.Min(lastDist, nextDist);
                    double sidedist = Math.Max(dist, mindist);

                    bool prioritize = dist > mindist;

                    if (sidedist < segmentDistance || sidedist == segmentDistance && prioritize && !prioritizedFound)
                    {
                        segmentDistance = sidedist;
                        prioritizedFound = prioritize;
                        segmentIndex = i;
                    }
                }
            }
        }

        public double SegmentDistance(Point point, int index)
        {
            if (SegmentSelected(point, index))
                return segmentDistance;
            return Double.PositiveInfinity;
        }

        public bool SegmentSelected(Point point, int index)
        {
            HitScan(point);
            return index == segmentDistance;
        }

        public Rect LineBounds(int index)
        {
            throw new NotImplementedException();
        }
    }
}
