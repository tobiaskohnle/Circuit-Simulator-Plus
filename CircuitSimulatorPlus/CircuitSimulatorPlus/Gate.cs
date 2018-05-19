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

        public List<Input> input = new List<Input>();
        public List<Output> output = new List<Output>();

        Point position;

        /// <summary>
        /// Triggered when any of the output signals changed.
        /// </summary>
        public EventHandler OutputChanged;

        /// <summary>
        /// Triggered when the Gate's position is changed.
        /// </summary>
        public EventHandler PositionChanged;

        /// <summary>
        /// Creates the Gate's visual representation on the Render() call.
        /// </summary>
        public IRenderer Renderer;

        public Point Position
        {
            get { return position; }

            set
            {
                position = value;
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
