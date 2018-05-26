using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        public bool mutable;
        public bool selected;
        public string name;
        public string tag;

        public List<InputNode> input = new List<InputNode>();
        public List<OutputNode> output = new List<OutputNode>();

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
        /// Render() should only be called once.
        /// </summary>
        public IRenderer Renderer;

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
