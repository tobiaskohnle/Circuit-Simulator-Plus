using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class Input : Connection
    {
        bool risingEdge;
        bool centered;
        List<ElementaryGate> connectTo;
    }
}
