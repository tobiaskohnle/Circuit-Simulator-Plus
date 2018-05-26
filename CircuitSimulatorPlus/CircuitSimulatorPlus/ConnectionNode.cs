using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CircuitSimulatorPlus
{
    [DebuggerDisplay("Id: {id}, Empty: {empty}, Inverted: {inverted}, Name: {name}, State==null: {state==null}, Repr==null: {repr==null}")]
    public abstract class ConnectionNode
    {
        public bool empty = true;
        public bool inverted;
        public string name;
        public int id;

        public ConnectionNode()
        {
            id = MainWindow.id++;
        }

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

        public abstract void Clear();
        public abstract void Invert();
    }
}
