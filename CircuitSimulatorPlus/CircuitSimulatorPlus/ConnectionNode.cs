using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ConnectionNode
    {
        public bool empty;
        public bool inverted;
        public string name;
        /// <summary>
        /// A reference to the ElementaryGate indicating the current state of this ConnectionNode
        /// </summary>
        public ElementaryGate state;
        /// <summary>
        /// Represents the ElementaryGate that is interacted with (i.e. used to create connections)
        /// </summary>
        public ElementaryGate repr;

        /// <summary>
        /// True, if this ConnectionNode is displayed as 'high' (1);
        /// False, if this ConnectionNode is displayed as 'low' (0)
        /// </summary>
        public bool State
        {
            get { return state.lastState; }
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
    }
}
