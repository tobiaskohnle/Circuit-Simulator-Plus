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
            IsEmpty = true;
            hitbox = new CircleHitbox(Position, HitboxRadius, DistanceFactor);
        }

        public const double HitboxRadius = 2.5;
        public const double DistanceFactor = 1;

        protected bool stateChanged;

        public Gate Owner;

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

        public virtual bool LogicState
        {
            get
            {
                return State;
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

        public void Invert()
        {
            IsInverted = !IsInverted;
        }

        protected void Tick(bool lastWasElementary, bool nextIsElementary, bool forceUpdate)
        {
            stateChanged = false;

            if (lastWasElementary)
            {
                State = Owner.Eval() != IsInverted;
            }
            else if (BackConnectedTo != null)
            {
                State = BackConnectedTo.LogicState != IsInverted;
            }
            else
            {
                State = IsInverted;
            }

            if (stateChanged || forceUpdate)
            {
                if (nextIsElementary)
                {
                    foreach (OutputNode node in Owner.Output)
                        MainWindow.Self.TickedNodes.Enqueue(node);
                }
                else
                {
                    foreach (ConnectionNode node in NextConnectedTo)
                        MainWindow.Self.TickedNodes.Enqueue(node);
                }
            }
        }

        public abstract void UpdatePosition(int index);

        public abstract void Tick();
        public abstract void ConnectTo(ConnectionNode connectionNode);
        public abstract void Clear();
    }
}
