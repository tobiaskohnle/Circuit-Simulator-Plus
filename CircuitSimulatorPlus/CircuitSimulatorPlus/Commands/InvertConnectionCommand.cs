using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class InvertConnectionCommand : Command
    {
        List<IClickable> Selected;

        public InvertConnectionCommand(List<IClickable> List) : base("Invert Connection")
        {
            Selected = List;
        }

        public override void Redo()
        {
            foreach (ConnectionNode item in Selected)
            {
                item.Invert();
            }
        }

        public override void Undo()
        {
            foreach (ConnectionNode item in Selected)
            {
                item.Invert();
            }
        }
    }

}
