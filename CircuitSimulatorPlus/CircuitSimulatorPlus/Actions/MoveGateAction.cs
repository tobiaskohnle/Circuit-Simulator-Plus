using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    /// <summary>
    /// A Gate is moved to another position by the user
    /// </summary>
    public class MoveGateAction : Action
    {
        Vector move;
        List<IClickable> movedGates;

        public MoveGateAction(List<IClickable> movedGates, Vector move, string message) : base(message)
        {
            this.move = move;
            this.movedGates = movedGates;
        }

        public override void Redo()
        {
            foreach (Gate gate in movedGates)
                gate.Move(move);
        }

        public override void Undo()
        {
            foreach (Gate gate in movedGates)
                gate.Move(-move);
        }
    }
}
