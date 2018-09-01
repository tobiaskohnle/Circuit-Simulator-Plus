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
        public AddInputCommand(Gate gate) : base("Added input to gate")
        {
            this.Gate = gate;
        }

        public override void Redo()
        {
            var inputNode = new InputNode(Gate);
            Gate.Input.Add(inputNode);
            MainWindow.Self.ClickableObjects.Add(inputNode);
        }

        public override void Undo()
        {
            var inputNode = new InputNode(Gate);
            Gate.Input.Remove(inputNode);
            MainWindow.Self.ClickableObjects.Remove(inputNode);
        }
    }
}
