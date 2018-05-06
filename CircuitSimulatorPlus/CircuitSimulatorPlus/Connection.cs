using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class Connection
    {
        public bool empty;
        public bool inverted;
        public string name;
        public ElementaryGate level;
    }
}
