using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
   public class CreateConnectionAction : Action
    {
        Cable Cable;
        public CreateConnectionAction(Cable cable) : base("Create Connection")
        {
            this.Cable = cable;
        }

        public override void Redo()
        {
            Cable.CreateCable();
        }

        public override void Undo()
        {
            Cable.DeleteCable();
        }
    }
}
