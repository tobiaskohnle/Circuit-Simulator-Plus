using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        Point position;
        Size size;
        string name;
        string tag;
        bool mutable;

        List<Input> input;
        List<Output> output;
    }
}
