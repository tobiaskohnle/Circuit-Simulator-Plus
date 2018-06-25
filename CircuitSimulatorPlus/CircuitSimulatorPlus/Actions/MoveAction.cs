using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class MoveAction : Action
    {
        Vector move;
        IMovable movedObject;

        public MoveAction(IMovable movedObject, Vector move) : base("Moved object")
        {
            this.move = move;
            this.movedObject = movedObject;
        }

        public override void Redo()
        {
            movedObject.Move(move);
        }

        public override void Undo()
        {
            movedObject.Move(-move);
        }
    }
}
