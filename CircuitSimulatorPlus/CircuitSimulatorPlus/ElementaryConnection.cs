using System;
using System.Collections.Generic;

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
