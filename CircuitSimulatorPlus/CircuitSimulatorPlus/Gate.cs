using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        bool mutable;
        string name;
        string tag;

        List<Input> input;
        List<Output> output;
    }
}
