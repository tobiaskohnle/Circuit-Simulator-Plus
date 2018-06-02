using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class CreateConnectionAction : Action
    {
        InputNode inputNode;
        OutputNode outputNode;

        public CreateConnectionAction(InputNode inputNode, OutputNode outputNode) : base("Created Connection")
        {
            this.inputNode = inputNode;
            this.outputNode = outputNode;
        }
        public override void Redo()
        {
            outputNode.ConnectTo(inputNode);
        }

        public override void Undo()
        {
            inputNode.Clear();
        }
    }
}
