using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ChangeConnectionAction : Action
    {
        //waiting for disconnect-function
        public ChangeConnectionAction(string message) : base(message)
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
