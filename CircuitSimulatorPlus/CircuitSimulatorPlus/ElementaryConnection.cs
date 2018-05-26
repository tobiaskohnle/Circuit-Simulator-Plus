using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class ElementaryConnection
    {
        protected ElementaryGate next;
        protected int index;

        public ElementaryConnection(ElementaryGate next, int index)
        {
            this.next = next;
            this.index = index;
        }

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
