using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Cable: IClickable, IMovable
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
    }
}
