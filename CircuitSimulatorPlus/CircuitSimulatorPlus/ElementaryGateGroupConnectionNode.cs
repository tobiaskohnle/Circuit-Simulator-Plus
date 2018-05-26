using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    class ElementaryGateGroupConnectionNode
    {
        /// <summary>
        /// A reference to the ElementaryGate indicating the current state of this ConnectionNode
        /// </summary>
        public ElementaryGate state;
        /// <summary>
        /// </summary>
        public ElementaryGate reprA;
        /// <summary>
        /// </summary>
        public ElementaryGate reprB;
    }
}
