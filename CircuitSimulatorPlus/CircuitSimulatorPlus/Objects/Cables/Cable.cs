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

        bool vertical;

        public List<double> Points;

        public Point StartPos
        {
            get
            {
                return OutputNode.Position;
            }
        }
        public Point EndPos
        {
            get
            {
                return InputNode.Position;
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

                for (int i = 0; i < Points.Count; i++)
                {
                    var point = Points[i];
                    var lastpoint = Points[i - 1];
                    var nextpoint = Points[i + 1];

                    var vert = (i & 1) != 0;

                    //var sx = vert ? point.x : lastpoint.x;
                    //var sy = vert ? lastpoint.y : point.y;
                    //var ex = vert ? point.x : nextpoint.x;
                    //var ey = vert ? nextpoint.y : point.y;

                    double sx = 0;
                    double sy = 0;
                    double ex = 0;
                    double ey = 0;

                    double dist = vert ? Math.Abs(pos.X - sx) : Math.Abs(pos.Y - sy);

                    double lastdist = vert ? Math.Abs(pos.Y - sy) : Math.Abs(pos.X - sx);
                    double nextdist = vert ? Math.Abs(pos.Y - ey) : Math.Abs(pos.X - ex);

                    var len = vert ? Math.Abs(ey - sy) : Math.Abs(ex - sx);

                    if (lastdist < len && nextdist < len)
                    {
                        if (dist < segmentDistance || dist == segmentDistance && !prioritizedFound)
                        {
                            segmentDistance = dist;
                            prioritizedFound = true;
                            segmentIndex = i;
                        }
                        continue;
                    }

                    double mindist = Math.Min(lastdist, nextdist);
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
    }
}
