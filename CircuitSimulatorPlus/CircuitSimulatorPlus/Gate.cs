using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate : IClickable
    {
        public const double DistanceFactor = 0.7;

        public Gate(GateType type = GateType.Context)
        {
            Type = type;
            HasContext = type == GateType.Context;
            Context = new List<Gate>();
            Input = new List<InputNode>();
            Output = new List<OutputNode>();
            hitbox = new RectHitbox(this, new Rect(), DistanceFactor);
            UpdateSize();
            UpdateHitbox();
        }

        public enum GateType
        {
            Context, And, Or, Identity
        }
        /// <summary>
        /// </summary>
        public GateType Type { get; private set; }
        /// <summary>
        /// The circuit inside of a gate.
        /// </summary>
        public List<Gate> Context { get; set; }
        /// <summary>
        /// False: elementary gate
        /// </summary>
        public bool HasContext { get; private set; }
        /// <summary>
        /// InputNodes of the gate.
        /// </summary>
        public List<InputNode> Input { get; set; }
        /// <summary>
        /// OutputNodes of the gate.
        /// </summary>
        public List<OutputNode> Output { get; set; }
        /// <summary>
        /// Triggered when any of the connection signals changed.
        /// </summary>
        public EventHandler ConnectionChanged { get; set; }
        /// <summary>
        /// </summary>
        public EventHandler ConnectionCreated { get; set; }
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
        /// <summary>
        /// The position on the canvas of the gate.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set {
                position = value;
                UpdateHitbox();
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public Size Size { get; private set; }

        public void UpdateSize()
        {
            Size = new Size(3, 4);
            UpdateHitbox();
        }

        void UpdateHitbox()
        {
            hitbox.Bounds = new Rect(Position, Size);
        }
        /// <summary>
        /// Name displayed on top of the gate.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// True, if you can add or remove inputs of this gate.
        /// </summary>
        public bool IsMutable { get; set; }
        /// <summary>
        /// True, if the gate is currently selected.
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Tag displayed inside of the gate. (e.g. ">=1" for OR)
        /// </summary>
        public string Tag { get; set; }

        RectHitbox hitbox;

        public Hitbox Hitbox
        {
            get { return hitbox; }
            set { hitbox = value as RectHitbox; }
        }

        /// <summary>
        /// Moves the gate.
        /// </summary>
        public void Move(Vector move)
        {
            Position = new Point(Position.X + move.X, Position.Y + move.Y);
        }
        /// <summary>
        /// </summary>
        public void SnapToGrid()
        {
            Position = new Point(Math.Round(Position.X), Math.Round(Position.Y));
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Eval()
        {
            switch (Type)
            {
            case GateType.And:
                foreach (InputNode input in Input)
                    if (input.State == false)
                        return false;
                return true;
            case GateType.Or:
                foreach (InputNode input in Input)
                    if (input.State)
                        return true;
                return false;
            case GateType.Identity:
                foreach (InputNode input in Input)
                    return input.State;
                return false;
            }
            throw new InvalidOperationException($"Can't Eval() gate of type {Type}.");
        }
    }
}
