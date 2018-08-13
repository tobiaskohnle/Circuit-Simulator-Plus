using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class InvertConnectionCommand : Command
    {
        List<ConnectionNode> Selected;
        public InvertConnectionCommand(List<ConnectionNode> List) : base("Invert Connection")
        {
            this.Selected = List;
        }

        public override void Redo()
        {
            foreach (ConnectionNode item in Selected)
            {
                item.IsInverted = true;
            }
        }

        public override void Undo()
        {
            foreach (ConnectionNode item in Selected)
            {
                item.IsInverted = false;
            }
        }
    }

}
