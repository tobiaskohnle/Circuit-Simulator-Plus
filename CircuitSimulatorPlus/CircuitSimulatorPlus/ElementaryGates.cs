using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return false;
        }
    }
}
