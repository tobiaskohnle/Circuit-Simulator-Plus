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
        IClickable movedObject;

        public MoveAction(IClickable movedObject, Vector move) : base("Moved object")
        {
            this.move = move;
            this.movedObject = movedObject;
        }

        public override void Redo()
        {
            if (movedObject is Gate)
                (movedObject as Gate).Move(move);
        }

        public override void Undo()
        {
            if (movedObject is Gate)
                (movedObject as Gate).Move(-move);
        }
    }
}
