using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class CreateConnectionAction : Action
    {
        ConnectionNode firstNode, secondNode;

        public CreateConnectionAction(ConnectionNode firstNode, ConnectionNode secondNode, string message) : base(message)
        {
            this.firstNode = firstNode;
            this.secondNode = secondNode;
        }
        public override void Redo()
        {
            firstNode.ConnectTo(secondNode);
        }

        public override void Undo()
        {
            //waiting for implementation of disconnect-function
        }
    }
}
