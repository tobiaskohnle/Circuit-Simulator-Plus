using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class ElementaryConnection
    {
        ElementaryGate next;
        int index;

        public ElementaryGate Next
        {
            get {
                return next;
            }
        }

        public int Index
        {
            get {
                return index;
            }
        }
    }
}
