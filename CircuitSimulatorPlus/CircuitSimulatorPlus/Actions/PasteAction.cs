using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class PasteAction : Action
    {
        public PasteAction(Gate gate) : base($"Pasted {gate.Type}-Gate")
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
