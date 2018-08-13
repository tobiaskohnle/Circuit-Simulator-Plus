using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class RenameGateCommand : Command
    {
        Gate gate;
        string oldName;
        string newName;

        public RenameGateCommand(Gate gate, string name) : base($"Renamed Gate to {name}")
        {
            this.gate = gate;
            newName = name;
            oldName = gate.Name;
        }

        public override void Redo()
        {
            gate.Name = oldName;
        }

        public override void Undo()
        {
            gate.Name = newName;
        }
    }
}
