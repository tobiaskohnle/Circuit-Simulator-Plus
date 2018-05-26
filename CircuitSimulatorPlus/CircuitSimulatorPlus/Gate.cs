using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        bool mutable;
        bool selected;
        string name;
        string tag;

        public List<InputNode> Input { get; set; }

        public List<OutputNode> Output { get; set; }

        public ElementaryGateGroup ElementaryGates { get; set; }

        /// <summary>
        /// Triggered when any of the output signals changed.
        /// </summary>
        public EventHandler OutputChanged { get; set; }

        /// <summary>
        /// Triggered when the Gate's position is changed.
        /// </summary>
        public EventHandler PositionChanged { get; set; }

        /// <summary>
        /// Creates the Gate's visual representation on the Render() call.
        /// Render() should only be called once.
        /// </summary>
        public IRenderer Renderer { get; set; }

        Point position;

        public Point Position
        {
            get { return position; }

            set {
                position = value;
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Move(double x, double y)
        {
            position.X += x;
            position.Y += y;
        }
    }
}
