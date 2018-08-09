using System;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Hitbox
    {
        public Hitbox(object attachedObject, double distanceFactor)
        {
            this.distanceFactor = distanceFactor;
            AttachedObject = attachedObject;
        }

        protected double distanceFactor;

        public object AttachedObject
        {
            get; protected set;
        }

        public abstract Rect RectBounds();

        public abstract bool IncludesPos(Point pos);

        public abstract bool IsIncludedIn(Rect rect);

        public abstract double DistanceTo(Point pos);
    }
}
