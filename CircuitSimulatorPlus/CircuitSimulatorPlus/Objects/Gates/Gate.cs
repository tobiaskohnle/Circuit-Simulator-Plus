﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class Gate : IClickable, IMovable
    {
        public Gate()
        {
            Renderer = new GateRenderer(this);

            hitbox = new RectHitbox(this, new Rect(), DistanceFactor);
            Size = new Size(3, 4);

            ConnectedNodes = new Dictionary<ConnectionNode.Align, List<ConnectionNode>>();
            foreach (ConnectionNode.Align align in Enum.GetValues(typeof(ConnectionNode.Align)))
            {
                ConnectedNodes[align] = new List<ConnectionNode>();
            }
        }

        public const double DistanceFactor = 0.2;

        public GateRenderer Renderer;

        public Dictionary<ConnectionNode.Align, List<ConnectionNode>> ConnectedNodes;

        public List<InputNode> Input = new List<InputNode>();
        public List<OutputNode> Output = new List<OutputNode>();

        public event System.Action OnNameChanged;
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

        public event System.Action OnTagChanged;
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
        
        public event System.Action OnSelectionChanged;
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
        
        public event System.Action OnPositionChanged;
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
        
        public event System.Action OnSizeChanged;
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