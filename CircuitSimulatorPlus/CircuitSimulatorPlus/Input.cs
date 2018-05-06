using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class Input : Connection
    {
        public bool risingEdge;
        public bool centered;
        public List<ElementaryGate> connectTo;
    }
}
