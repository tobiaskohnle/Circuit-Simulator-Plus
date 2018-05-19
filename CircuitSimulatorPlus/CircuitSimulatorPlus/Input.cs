using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class Input : ConnectionNode
    {
        public bool risingEdge;
        public bool centered;
        public List<ElementaryGate> connectTo;
    }
}
