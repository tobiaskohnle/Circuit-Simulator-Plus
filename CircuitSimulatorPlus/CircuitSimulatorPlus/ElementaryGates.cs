using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public class AndElementaryGate : ElementaryGate
    {
        protected override bool Update()
        {
            foreach (bool s in state)
                if (s == false)
                    return false;
            return true;
        }
    }

    public class OrElementaryGate : ElementaryGate
    {
        protected override bool Update()
        {
            foreach (bool s in state)
                if (s)
                    return true;
            return false;
        }
    }

    public class NotElementaryGate : ElementaryGate
    {
        protected override bool Update()
        {
            foreach (bool s in state)
                return s == false;
            return true;
        }
    }

    public class RisingEdgeElementaryGate : ElementaryGate
    {
        protected override bool Update()
        {
            return true;
        }
    }
}
