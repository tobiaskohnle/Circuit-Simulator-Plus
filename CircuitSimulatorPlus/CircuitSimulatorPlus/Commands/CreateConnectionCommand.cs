using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class CreateConnectionCommand : Command
    {
        Cable Cable;
        public CreateConnectionCommand(Cable cable) : base("Create Connection")
        {
            Cable = cable;
        }

        public override void Redo()
        {
        }

        public override void Undo()
        {
        }
    }
}
