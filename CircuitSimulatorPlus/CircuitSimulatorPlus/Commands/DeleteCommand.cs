using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class DeleteCommand : Command
    {
        IClickable deletedObject;

        public DeleteCommand(IClickable deletedObject) : base("Deleted object")
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
