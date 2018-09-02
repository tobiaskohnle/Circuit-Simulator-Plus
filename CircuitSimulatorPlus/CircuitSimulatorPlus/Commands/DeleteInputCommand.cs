using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    class DeleteInputCommand : Command
    {
        InputNode inputNode;
        public DeleteInputCommand(InputNode inputNode) : base("Delete Input")
        {
            this.inputNode = inputNode;
        }

        public override void Redo()
        {
            inputNode.Owner.Input.Remove(inputNode);
            MainWindow.Self.ClickableObjects.Remove(inputNode);
        }

        public override void Undo()
        {
            inputNode.Owner.Input.Add(inputNode);
            MainWindow.Self.ClickableObjects.Add(inputNode);
        }
    }
}
