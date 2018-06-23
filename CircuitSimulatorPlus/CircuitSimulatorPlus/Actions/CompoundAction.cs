using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class CompoundAction : Action
    {
        public List<Action> Actions = new List<Action>();

        public CompoundAction() : base(null)
        {
        }

        public override void Redo()
        {
            foreach (Action action in Actions)
                action.Redo();
        }

        public override void Undo()
        {
            foreach (Action action in Actions)
                action.Undo();
        }
    }
}
