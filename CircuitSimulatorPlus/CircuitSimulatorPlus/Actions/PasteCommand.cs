using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class PasteCommand : Command
    {
        public PasteCommand(Gate gate) : base($"Pasted {gate.GetType()}")
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
