using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    class DeleteInputCommand : Command
    {
        Gate Gate;
        InputNode InputNode;
        public DeleteInputCommand(Gate gate,InputNode inputNode) : base("Delete Input")
        {
            this.Gate = gate;
            this.InputNode = inputNode;
        }

        public override void Redo()
        {
            Gate.Input.Remove(InputNode);
            MainWindow.Self.ClickableObjects.Remove(InputNode);
        }

        public override void Undo()
        {
            Gate.Input.Add(InputNode);
            MainWindow.Self.ClickableObjects.Add(InputNode);
        }
    }
}
