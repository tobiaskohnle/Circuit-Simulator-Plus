using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Gate : IClickable, IMovable
    {
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
                if (hitbox != null)
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
                if (hitbox != null)
                    hitbox.Bounds.Size = value;
                UpdateConnectionNodePos();
                OnSizeChanged?.Invoke();
            }
        }

        public void RemoveInputNode(InputNode inputNode)
        {
            Input.Remove(inputNode);
            inputNode.Remove();
            UpdateConnectionNodePos();
        }
        public void RemoveOutputNode(OutputNode outputNode)
        {
            Output.Remove(outputNode);
            outputNode.Remove();
            UpdateConnectionNodePos();
        }

        public void AddEmptyInputNode()
        {
            var inputNode = new InputNode(this);
            Input.Add(inputNode);
            inputNode.Add();
            UpdateConnectionNodePos();
        }
        public void AddEmptyOutputNode()
        {
            var outputNode = new OutputNode(this);
            Output.Add(outputNode);
            outputNode.Add();
            UpdateConnectionNodePos();
        }

        RectHitbox hitbox;
        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
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

        public abstract void CreateDefaultConnectionNodes();

        public void CreateConnectionNodes(int amtInputs, int amtOutputs)
        {
            for (int i = 0; i < amtInputs; i++)
            {
                Input.Add(new InputNode(this));
            }
            for (int i = 0; i < amtOutputs; i++)
            {
                Output.Add(new OutputNode(this));
            }
            UpdateConnectionNodePos();
        }

        public virtual void Add()
        {
            hitbox = new RectHitbox(new Rect(Position, Size));
            MainWindow.Self.ClickableObjects.Add(this);
            if (!MainWindow.Self.ContextGate.Context.Contains(this))//temp
                MainWindow.Self.ContextGate.Context.Add(this);
            MainWindow.Self.refs.Add(new WeakReference<IClickable>(this));//temp
            new GateRenderer(this);
            IsRendered = true;

            foreach (InputNode inputNode in Input)
                inputNode.Add();
            foreach (OutputNode outputNode in Output)
                outputNode.Add();

            UpdateConnectionNodePos();
        }
        public virtual void Remove()
        {
            foreach (InputNode input in Input)
                input.Remove();
            foreach (OutputNode output in Output)
                output.Remove();

            IsRendered = false;
            MainWindow.Self.ClickableObjects.Remove(this);
            MainWindow.Self.ContextGate.Context.Remove(this);
        }
    }
}
