using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ClearOutputNodeAction : Action
    {
        OutputNode clearedNode;
        List<InputNode> wasConnectedTo;

        public ClearOutputNodeAction(OutputNode clearedNode) : base("Cleared Output Node")
        {
            this.clearedNode = clearedNode;
            wasConnectedTo = clearedNode.NextConnectedTo.Cast<InputNode>().ToList();
        }

        public override void Redo()
        {
            clearedNode.Clear();
        }

        public override void Undo()
        {
            foreach (InputNode input in wasConnectedTo)
                clearedNode.ConnectTo(input);
        }
    }
}
