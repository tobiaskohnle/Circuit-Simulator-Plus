using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ConnectionNode
    {
        public List<ConnectionNode> NextConnectedTo { get; set; }
        public ConnectionNode BackConnectedTo { get; set; }

        bool state;
        bool stateChanged;
        /// <summary>
        /// True, if this ConnectionNode is displayed as 'high' (1).
        /// False, if this ConnectionNode is displayed as 'low' (0).
        /// </summary>
        public bool State
        {
            get { return state; }
            set {
                if (state != value)
                {
                    stateChanged = !stateChanged;
                    state = value;
                }
            }
        }
        /// <summary>
        /// True, if this ConnectionNode is NOT connected to another ConnectionNode.
        /// </summary>
        public bool IsEmpty { get; set; }
        /// <summary>
        /// True, if this ConnectionNode is inverted.
        /// </summary>
        public bool IsInverted { get; set; }
        /// <summary>
        /// Name displayed next to the ConnectionNode.
        /// (inside the Gate it is connected to)
        /// </summary>
        public string Name { get; set; }

        public abstract void Clear();

        /// <summary>
        /// Inverts the ConnectionNode.
        /// </summary>
        public void Invert()
        {
            IsInverted = !IsInverted;
        }

        public void Tick(Queue<ConnectionNode> tickedNodes)
        {
            bool nextIsElementary = false;
            bool lastWasElementary = false;
            Gate self = null;

            if (IsEmpty)
            {
                if (lastWasElementary)
                {
                    State = self.Eval();
                }
            }
            else
            {
                State = BackConnectedTo.State != IsInverted;
            }

            if (stateChanged)
            {
                stateChanged = false;

                if (nextIsElementary)
                {
                    foreach (OutputNode output in self.Output)
                    {
                        foreach (ConnectionNode node in output.NextConnectedTo)
                        {
                            tickedNodes.Enqueue(node);
                        }
                    }
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
