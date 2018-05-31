using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
   public class GateCreateAction : Action
    {
        Gate gate;
        Gate.GateType gateType;
        Point position;

        public GateCreateAction(Gate gate, Gate.GateType gateType)
        {
            this.gate = gate;
            this.gateType = gateType;
        }
         public override void Redo()
        {
            
        }

        public override void Undo()
        {

        }
    }
}
