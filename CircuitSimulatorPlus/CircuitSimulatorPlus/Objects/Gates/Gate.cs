using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Gate : IClickable, IMovable
    {
        public Gate(int amtInputs, int amtOutputs)
        {
            hitbox = new RectHitbox(new Rect());
            Size = new Size(3, 4);

            for (int i = 0; i < amtInputs; i++)
            {
                var inputNode = new InputNode(this);
                Input.Add(inputNode);
                MainWindow.Self.ClickableObjects.Add(inputNode);
            }
            for (int i = 0; i < amtOutputs; i++)
            {
                var outputNode = new OutputNode(this);
                Output.Add(outputNode);
                MainWindow.Self.ClickableObjects.Add(outputNode);
            }

            new GateRenderer(this);
        }

        public void CopyFrom(Gate gate)
        {
            Input = gate.Input;
            foreach (InputNode inputNode in Input)
                inputNode.Owner = this;

            Output = gate.Output;
            foreach (OutputNode outputNode in Output)
                outputNode.Owner = this;

            Name = gate.Name;
            //Tag = gate.Tag;
            Position = gate.Position;
            Size = gate.Size;
        }

        public const double DistanceFactor = 0.2;

        public List<InputNode> Input = new List<InputNode>();
        public List<OutputNode> Output = new List<OutputNode>();

        public event Action OnRenderedChanged;
        protected bool isRendered;
        public bool IsRendered
        {
            get
            {
                return isRendered;
            }
            set
            {
                if (isRendered != value)
                {
                    isRendered = value;
                    OnRenderedChanged?.Invoke();
                }
            }
        }

        public event Action OnNameChanged;
        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnNameChanged?.Invoke();
            }
        }

        public event Action OnTagChanged;
        string tag;
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
                OnTagChanged?.Invoke();
            }
        }

        public event Action OnSelectionChanged;
        bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnSelectionChanged?.Invoke();
            }
        }

        public event Action OnPositionChanged;
        Point position;
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                hitbox.Bounds.Location = value;
                UpdateConnectionNodePos();
                OnPositionChanged?.Invoke();
            }
        }

        public void UpdateConnectionNodePos()
        {
            for (int i = 0; i < Input.Count; i++)
            {
                Input[i].UpdatePosition(i);
            }
            for (int i = 0; i < Output.Count; i++)
            {
                Output[i].UpdatePosition(i);
            }
        }

        public event Action OnSizeChanged;
        Size size;
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                hitbox.Bounds.Size = value;
                UpdateConnectionNodePos();
                OnSizeChanged?.Invoke();
            }
        }

        public void RemoveInputNode(InputNode inputNode)
        {
            Input.Remove(inputNode);
            inputNode.IsRendered = false;
        }
        public void RemoveOutputNode(OutputNode outputNode)
        {
            Output.Remove(outputNode);
            outputNode.IsRendered = false;
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

        public void Move(Vector vector)
        {
            Position += vector;
        }

        public virtual bool HasContext
        {
            get
            {
                return false;
            }
        }

        public abstract bool Eval();
    }
}
