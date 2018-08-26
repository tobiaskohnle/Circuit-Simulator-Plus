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

        public const double HitboxRadius = 2.5;
        public const double DistanceFactor = 1;

        public enum Align
        {
            U, D, L, R
        }

        bool stateChanged;

        public Gate Owner;

        public Vector AlignmentVector = new Vector();

        public ConnectionNode BackConnectedTo
        {
            get; set;
        }
        public List<ConnectionNode> NextConnectedTo { get; set; } = new List<ConnectionNode>();

        public event Action OnTickedChanged;
        protected bool isTicked;
        public bool IsTicked
        {
            get
            {
                return isTicked;
            }
            set
            {
                isTicked = value;
                OnTickedChanged?.Invoke();
            }
        }

        public event Action OnEmptyChanged;
        protected bool isEmpty;
        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
            set
            {
                isEmpty = value;
                OnEmptyChanged?.Invoke();
            }
        }

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

        public event Action OnStateChanged;
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
                    OnStateChanged?.Invoke();
                }
            }
        }

        public event Action OnInvertedChanged;
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
                    hitbox.Center = value;
                OnPositionChanged?.Invoke();
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

        public event Action OnAlignmentChanged;
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
                    Owner.AmtConnectedNodes[(Align)alignment]--;
                Owner.AmtConnectedNodes[value]++;

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

                OnAlignmentChanged?.Invoke();
            }
        }

        public void Invert()
        {
            IsInverted = !IsInverted;
        }
        protected void Tick(Queue<ConnectionNode> tickedNodes, bool lastWasElementary, bool nextIsElementary)
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
            double sidePos = sideLength * (1 + 2 * index) / (2 * Owner.AmtConnectedNodes[Alignment]);

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
