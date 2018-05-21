using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    /// <summary>
    /// A Gate is moved to another position by the user
    /// </summary>
    public class MoveGateAction : Action
    {
        double moveX, moveY;
        List<Gate> movedGates;

        public MoveGateAction(List<Gate> movedGates, double moveX, double moveY, string message) : base(message)
        {
            this.moveX = moveX;
            this.moveY = moveY;
            this.movedGates = movedGates;
        }

        public override void Redo()
        {
            foreach (Gate gate in movedGates)
                gate.Move(moveX, moveY);
        }

        public override void Undo()
        {
            foreach (Gate gate in movedGates)
                gate.Move(-moveX, -moveY);
        }
    }
}
