using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class MoveCommand : Command
    {
        Vector move;
        List<IMovable> movedObjects;

        public MoveCommand(List<IMovable> movedObjects, Vector move) : base("Moved objects")
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
