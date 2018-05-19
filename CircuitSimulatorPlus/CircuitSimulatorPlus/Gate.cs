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

        public List<Input> input = new List<Input>();
        public List<Output> output = new List<Output>();

        /// <summary>
        /// Triggered when any of the output signals changed.
        /// </summary>
        public EventHandler OutputChanged;
    }
}
