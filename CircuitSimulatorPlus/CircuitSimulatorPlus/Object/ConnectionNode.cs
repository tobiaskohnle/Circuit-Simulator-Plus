using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class ConnectionNode : IClickable
    {
        protected ConnectionNode(Gate owner)
        {
            Owner = owner;
            hitbox = new CircleHitbox(this, Position, HitboxRadius, DistanceFactor);
        }

        public const double HitboxRadius = 2.5;
        public const double DistanceFactor = 1;

        bool state;
        bool inverted;
        bool stateChanged;
        bool isSelected;
        Point position;
        string name;
        CircleHitbox hitbox;

        public List<ConnectionNode> NextConnectedTo { get; set; } = new List<ConnectionNode>();
        public ConnectionNode BackConnectedTo
        {
            get; set;
        }

        /// <summary>
        /// True, if this ConnectionNode is displayed as 'high' (1).
        /// False, if this ConnectionNode is displayed as 'low' (0).
        /// </summary>
        public bool State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    stateChanged = !stateChanged;
                    state = value;
                    Renderer.OnStateChanged();
                }
            }
        }

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                hitbox.Center = value;
                Renderer.OnPositionChanged();
            }
        }

        /// <summary>
        /// A reference to the Gate which the ConnectionNode is connected to.
        /// </summary>
        public Gate Owner
        {
            get; private set;
        }

        /// <summary>
        /// True, if this ConnectionNode is NOT connected to another ConnectionNode.
        /// </summary>
        public bool IsEmpty { get; set; } = true;

        /// <summary>
        /// True, if this ConnectionNode is inverted.
        /// </summary>
        public bool IsInverted
        {
            get
            {
                return inverted;
            }
            set
            {
                inverted = value;
                Renderer.OnInvertedChanged();
            }
        }

        /// <summary>
        /// Name displayed next to the ConnectionNode.
        /// (inside the Gate it is connected to)
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

        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
            }
            set
            {
                hitbox = value as CircleHitbox;
            }
        }

        public ConnectionNodeRenderer Renderer
        {
            get; set;
        }

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

        public bool IsMovable
        {
            get
            {
                return false;
            }
        }

        public abstract void Clear();

        /// <summary>
        /// Inverts the ConnectionNode.
        /// </summary>
        public void Invert()
        {
            IsInverted = !IsInverted;
        }

        public virtual void ConnectTo(ConnectionNode connectionNode)
        {
            NextConnectedTo.Add(connectionNode);
            connectionNode.BackConnectedTo = this;
            Owner.Renderer.OnPositionChanged();
        }

        public abstract void Tick(Queue<ConnectionNode> tickedNodes);

        protected void Tick(Queue<ConnectionNode> tickedNodes, bool nextIsElementary, bool lastWasElementary)
        {
            stateChanged = false;

            if (lastWasElementary)
            {
                State = Owner.Eval();
            }
            else if (BackConnectedTo != null)
            {
                State = BackConnectedTo.State;
            }
            else
            {
                State = false;
            }

            if (IsInverted)
            {
                State = !State;
            }

            if (stateChanged)
            {
                stateChanged = false;

                if (nextIsElementary)
                {
                    foreach (OutputNode node in Owner.Output)
                        tickedNodes.Enqueue(node);
                }
                else
                {
                    foreach (ConnectionNode node in NextConnectedTo)
                        tickedNodes.Enqueue(node);
                }
            }
        }
    }
}
