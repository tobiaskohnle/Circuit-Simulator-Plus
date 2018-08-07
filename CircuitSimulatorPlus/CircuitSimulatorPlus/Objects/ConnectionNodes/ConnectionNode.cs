using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public abstract class ConnectionNode : IClickable
    {
        protected ConnectionNode(Align alignment, Gate owner)
        {
            Owner = owner;
            Alignment = alignment;
            IsEmpty = true;
            hitbox = new CircleHitbox(this, Position, HitboxRadius, DistanceFactor);
        }

        public ConnectionNodeRenderer Renderer;

        public const double HitboxRadius = 2.5;
        public const double DistanceFactor = 1;

        public enum Align
        {
            U, D, L, R
        }

        bool stateChanged;

        public Gate Owner;
        public bool IsEmpty;

        public Vector AlignmentVector = new Vector();

        public ConnectionNode BackConnectedTo
        {
            get; set;
        }
        public List<ConnectionNode> NextConnectedTo { get; set; } = new List<ConnectionNode>();

        public event System.Action OnStateChanged;
        bool state;
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
                }
                OnStateChanged?.Invoke();
            }
        }
        
        public event System.Action OnInvertedChanged;
        bool inverted;
        public bool IsInverted
        {
            get
            {
                return inverted;
            }
            set
            {
                inverted = value;
                OnInvertedChanged?.Invoke();
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
                if (hitbox != null)
                    hitbox.Center = value;
                OnPositionChanged?.Invoke();
            }
        }
        
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

        CircleHitbox hitbox;
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

        public event System.Action OnAlignmentChanged;
        Align? alignment;
        public Align Alignment
        {
            get
            {
                return (Align)alignment;
            }
            set
            {
                if (alignment != null)
                    Owner.ConnectedNodes[(Align)alignment].Remove(this);
                Owner.ConnectedNodes[value].Add(this);

                switch (alignment = value)
                {
                case Align.U:
                    AlignmentVector.X = 0;
                    AlignmentVector.Y = -1;
                    break;
                case Align.R:
                    AlignmentVector.X = 1;
                    AlignmentVector.Y = 0;
                    break;
                case Align.D:
                    AlignmentVector.X = 0;
                    AlignmentVector.Y = 1;
                    break;
                case Align.L:
                    AlignmentVector.X = -1;
                    AlignmentVector.Y = 0;
                    break;
                }

                Owner.UpdateConnectionNodePos();
                OnAlignmentChanged?.Invoke();
            }
        }

        public void Invert()
        {
            IsInverted = !IsInverted;
        }
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
        public void UpdatePosition(int index)
        {
            double sideLength = alignment == Align.U || alignment == Align.D ? Owner.Size.Width : Owner.Size.Height;
            double sidePos = sideLength * (1 + 2 * index) / (2 * Owner.ConnectedNodes[Alignment].Count);

            Position = new Point(
                Owner.Position.X + Owner.Size.Width / 2
                    + AlignmentVector.X * Owner.Size.Width / 2
                    - AlignmentVector.Y * (sidePos - Owner.Size.Width / 2),
                Owner.Position.Y + Owner.Size.Height / 2
                    + AlignmentVector.Y * Owner.Size.Height / 2
                    + AlignmentVector.X * (sidePos - Owner.Size.Height / 2)
            );
        }

        public abstract void Tick(Queue<ConnectionNode> tickedNodes);
        public abstract void ConnectTo(ConnectionNode connectionNode);
        public abstract void Clear();
    }
}
