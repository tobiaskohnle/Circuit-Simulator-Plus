using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ConnectionNode
    {
        protected bool empty = true;
        protected bool inverted;
        protected string name;

        protected List<InputNode> connectedTo = new List<InputNode>();

        /// <summary>
        /// True, if this ConnectionNode is displayed as 'high' (1);
        /// False, if this ConnectionNode is displayed as 'low' (0)
        /// </summary>
        public bool State
        {
            get { return false/*state.lastState*/; }
        }
        /// <summary>
        /// True, if this ConnectionNode is NOT connected to another ConnectionNode
        /// </summary>
        public bool IsEmpty
        {
            get { return empty; }
        }
        /// <summary>
        /// True, if this ConnectionNode is displayed as inverted
        /// </summary>
        public bool IsInverted
        {
            get { return inverted; }
        }
        /// <summary>
        /// Name displayed next to the ConnectionNode (inside the Gate it is connected to)
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        public abstract void Clear();

        public void Invert()
        {
            inverted = !inverted;
        }
    }
}
