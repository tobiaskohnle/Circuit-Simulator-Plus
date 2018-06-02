using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ClearInputNodeAction : Action
    {
        InputNode clearedNode;
        OutputNode wasConnectedTo;

        public ClearInputNodeAction(InputNode clearedNode) : base("Cleared Input Node")
        {
            this.clearedNode = clearedNode;
            wasConnectedTo = (OutputNode)clearedNode.BackConnectedTo;
        }

        public override void Redo()
        {
            clearedNode.Clear();
        }

        public override void Undo()
        {
            wasConnectedTo.ConnectTo(clearedNode);
        }
    }
}
