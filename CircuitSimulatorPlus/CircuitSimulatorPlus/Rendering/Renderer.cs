using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public abstract class Renderer<T>
    {
        public delegate void ValueChanged(T state);
    }
}
