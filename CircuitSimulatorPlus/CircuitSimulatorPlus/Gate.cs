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

        /// <summary>
        /// InputNodes of the gate
        /// </summary>
        public List<InputNode> Input { get; set; }

        /// <summary>
        /// OutputNodes of the gate
        /// </summary>
        public List<OutputNode> Output { get; set; }

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

        /// <summary>
        /// Name displayed on top of the gate
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// True, if you can add or remove inputs of this gate
        /// </summary>
        public bool IsMutable
        {
            get { return mutable; }
        }

        /// <summary>
        /// True, if the gate is currently selected
        /// </summary>
        public bool IsSelected
        {
            get { return selected; }
        }

        /// <summary>
        /// Tag displayed inside of the gate (e.g ">=1" for OR)
        /// </summary>
        public string Tag
        {
            get { return tag; }
        }

        /// <summary>
        /// Moves the gate
        /// </summary>
        /// <param name="x">Offset along x-axis in units</param>
        /// <param name="y">Offset along y-axis in units</param>
        public void Move(double x, double y)
        {
            position.X += x;
            position.Y += y;
        }
    }
}
