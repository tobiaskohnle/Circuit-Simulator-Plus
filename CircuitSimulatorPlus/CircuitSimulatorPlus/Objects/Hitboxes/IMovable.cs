using System.Windows;

namespace CircuitSimulatorPlus
{
    public interface IMovable
    {
        void Move(Vector vector);
        Point Position
        {
            get;
        }

        double SnapSize
        {
            get;
        }
    }
}
