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
        List<IMovable> movedObjects;

        public MoveAction(List<IMovable> movedObjects, Vector move) : base("Moved object")
        {
            this.move = move;
            this.movedObjects = movedObjects;
        }

        public override void Redo()
        {
            foreach (IMovable obj in movedObjects)
            {
                obj.Move(move);
            }
        }

        public override void Undo()
        {
            foreach (IMovable obj in movedObjects)
            {
                obj.Move(-move);
            }
        }
    }
}
