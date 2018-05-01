using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
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
}
