using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Gate : IClickable, IMovable
    {
        public Gate()
        {
            Input = new List<InputNode>();
            Output = new List<OutputNode>();
            hitbox = new RectHitbox(this, new Rect(), DistanceFactor);
            UpdateSize();
        }

        public const double DistanceFactor = 0.2;

        string name;
        string tag;
        Size size;

        public abstract string Type
        {
            get;
        }

        /// <summary>
        /// InputNodes of the gate.
        /// </summary>
        public List<InputNode> Input
        {
            get; set;
        }
        /// <summary>
        /// OutputNodes of the gate.
        /// </summary>
        public List<OutputNode> Output
        {
            get; set;
        }
        /// <summary>
        /// Creates the Gate's visual representation on the Render() call.
        /// Render() should only be called once.
        /// </summary>
        public GateRenderer Renderer
        {
            get; set;
        }

        Point position;
        /// <summary>
        /// The position on the canvas of the gate.
        /// </summary>
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                UpdateHitbox();
                Renderer.OnPositionChanged();
            }
        }
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                //Renderer.OnSizeChanged();
            }
        }

        public void UpdateConnectionNodePos()
        {
            for (int i = 0; i < Input.Count; i++)
            {
                Input[i].Position = new Point(Position.X, Position.Y
                    + Size.Height * (1 + 2 * i) / (2 * Input.Count));
            }
            for (int i = 0; i < Output.Count; i++)
            {
                Output[i].Position = new Point(Position.X + Size.Width, Position.Y
                    + Size.Height * (1 + 2 * i) / (2 * Output.Count));
            }
        }

        public void UpdateSize()
        {
            Size = new Size(3, 4);
            UpdateHitbox();
        }

        public void UpdateHitbox()
        {
            hitbox.Bounds = new Rect(Position, Size);
        }
        /// <summary>
        /// Name displayed on top of the gate.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                Renderer.OnNameChanged();
            }
        }
        /// <summary>
        /// True, if you can add or remove inputs of this gate.
        /// </summary>
        public bool IsMutable
        {
            get; set;
        }

        bool isSelected;
        /// <summary>
        /// True, if the gate is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                Renderer.OnSelectionChanged();
            }
        }
        /// <summary>
        /// Tag displayed inside of the gate. (e.g. ">=1" for OR)
        /// </summary>
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
                //Renderer.OnTagChanged();
            }
        }

        RectHitbox hitbox;

        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
            }
            set
            {
                hitbox = value as RectHitbox;
            }
        }

        public virtual bool HasContext
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Moves the gate.
        /// </summary>
        public void Move(Vector vector)
        {
            //Position = new Point(Position.X + vector.X, Position.Y + vector.Y);
            Position += vector;
            UpdateConnectionNodePos();
        }
        /// <summary>
        /// </summary>
        public abstract bool Eval();
    }
}
