using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class DeleteAction : Action
    {
        IClickable deletedObject;

        public DeleteAction(IClickable deletedObject) : base("Deleted object")
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
