using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        public bool mutable;
        public string name;
        public string tag;
        public Point position;

        public List<Input> input;
        public List<Output> output;
    }
}
