using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ChangeTypeCommand : Command
    {
        public ChangeTypeCommand(Gate gate, Gate newGate) : base($"Changed Type to {newGate.GetType()}")
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
