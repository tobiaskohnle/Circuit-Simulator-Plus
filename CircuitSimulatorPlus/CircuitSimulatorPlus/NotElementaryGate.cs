using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class NotElementaryGate : ElementaryGate
    {
        public override bool Update()
        {
            foreach (bool s in inputStates)
                return s == false;
            return true;
        }
    }
}
