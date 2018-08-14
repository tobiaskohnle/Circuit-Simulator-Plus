﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Gate : IClickable, IMovable
    {
        public Gate(int amtInputs, int amtOutputs)
        {
            hitbox = new RectHitbox(this, new Rect(), DistanceFactor);
            Size = new Size(3, 4);

            ConnectedNodes = new Dictionary<ConnectionNode.Align, List<ConnectionNode>>();
            foreach (ConnectionNode.Align align in Enum.GetValues(typeof(ConnectionNode.Align)))
            {
                ConnectedNodes[align] = new List<ConnectionNode>();
            }

            for (int i = 0; i < amtInputs; i++)
            {
                var inputNode = new InputNode(this);
                Input.Add(inputNode);
                MainWindow.ClickableObjects.Add(inputNode);
            }
            for (int i = 0; i < amtOutputs; i++)
            {
                var outputNode = new OutputNode(this);
                Output.Add(outputNode);
                MainWindow.ClickableObjects.Add(outputNode);
            }

            Renderer = new GateRenderer(this);
            if (GetType() != typeof(ContextGate))
            {
                IsRendered = true;
            }
        }

        public const double DistanceFactor = 0.2;

        public GateRenderer Renderer;

        public Dictionary<ConnectionNode.Align, List<ConnectionNode>> ConnectedNodes;

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
                OnPositionChanged?.Invoke();
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
                OnSizeChanged?.Invoke();
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

        public void Move(Vector vector)
        {
            Position += vector;
            UpdateConnectionNodePos();
        }
        public void UpdateConnectionNodePos()
        {
            foreach (var connectedNodes in ConnectedNodes.Values)
            {
                for (int i = 0; i < connectedNodes.Count; i++)
                {
                    connectedNodes[i].UpdatePosition(i);
                }
            }
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
