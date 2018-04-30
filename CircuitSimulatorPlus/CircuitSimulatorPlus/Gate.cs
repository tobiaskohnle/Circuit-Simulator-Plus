using System;
using System.Collections.Generic;
using System.Windows;

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
