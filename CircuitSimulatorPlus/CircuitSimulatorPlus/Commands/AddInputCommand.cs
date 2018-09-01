using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class AddInputCommand : Command
    {
        Gate Gate;
        InputNode InputNode;
        public AddInputCommand(Gate gate) : base("Added input to gate")
        {
            this.Gate = gate;
        }

        public override void Redo()
        {
            InputNode = new InputNode(Gate);
            Gate.Input.Add(InputNode);
            MainWindow.Self.ClickableObjects.Add(InputNode);
        }

        public override void Undo()
        {
            Gate.Input.Remove(InputNode);
            MainWindow.Self.ClickableObjects.Remove(InputNode);
        }
    }
}
