using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus.Actions
{
    class DeleteInputCommand : Command
    {
        Gate Gate;
        public DeleteInputCommand(Gate gate) : base("Delete Input")
        {
            this.Gate = gate;
        }

        public override void Redo()
        {
            var inputNode = new InputNode(Gate);
            Gate.Input.Remove(inputNode);
            MainWindow.Self.ClickableObjects.Remove(inputNode);
        }

        public override void Undo()
        {
            var inputNode = new InputNode(Gate);
            Gate.Input.Add(inputNode);
            MainWindow.Self.ClickableObjects.Add(inputNode);
        }
    }
}
