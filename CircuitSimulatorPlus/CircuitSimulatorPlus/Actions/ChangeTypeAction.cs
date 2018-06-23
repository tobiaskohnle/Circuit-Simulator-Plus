using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ChangeTypeAction : Action
    {
        public ChangeTypeAction(Gate gate, Gate newGate) : base($"Changed Type to {newGate.Type}")
        {

        }

        public override void Redo()
        {

        }

        public override void Undo()
        {

        }
    }
}
