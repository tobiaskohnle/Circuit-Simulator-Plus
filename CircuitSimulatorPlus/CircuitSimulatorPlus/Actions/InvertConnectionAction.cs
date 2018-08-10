using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class InvertConnectionAction : Action
    {
        List<ConnectionNode> Selected;
        public InvertConnectionAction(List<ConnectionNode> List) : base("Invert Connection")
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
