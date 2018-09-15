using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Hitbox
    {
        public abstract Rect RectBounds
        {
            get;
        }

        public abstract bool IncludesPos(Point pos);

        public abstract bool IsIncludedIn(Rect rect);

        public abstract double DistanceTo(Point pos);
    }
}
