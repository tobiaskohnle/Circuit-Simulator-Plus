using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class MoveObjectAction : Action
    {
        Vector move;
        List<IClickable> movedObjects;

        public MoveObjectAction(List<IClickable> movedObjects, Vector move) : base("Moved Gates")
        {
            this.move = move;
            this.movedObjects = movedObjects;
        }

        public override void Redo()
        {
            foreach (IClickable obj in movedObjects)
                if (obj is Gate)
                    (obj as Gate).Move(move);
        }

        public override void Undo()
        {
            foreach (IClickable obj in movedObjects)
                if (obj is Gate)
                    (obj as Gate).Move(-move);
        }
    }
}
