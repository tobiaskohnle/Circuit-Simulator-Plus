using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class OrElementaryGate : ElementaryGate
    {
        public override bool Update()
        {
            foreach (bool s in inputStates)
                if (s)
                    return true;
            return false;
        }
    }
}
