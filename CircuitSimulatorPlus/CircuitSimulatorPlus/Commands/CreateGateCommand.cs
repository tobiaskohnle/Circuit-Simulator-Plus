using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CreateGateCommand : Command
    {
        Gate createdGate;

        public CreateGateCommand(Gate createdGate) : base($"Created {createdGate.GetType()}")
        {
            this.createdGate = createdGate;
        }

        public override void Redo()
        {
            createdGate.Add();
        }

        public override void Undo()
        {
            createdGate.Remove();
        }
    }
}
