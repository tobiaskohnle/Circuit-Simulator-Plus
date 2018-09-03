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
        List<Vector> move;
        List<IMovable> movedObjects;

        public MoveCommand(List<IMovable> movedObjects, List<Vector> move) : base("Moved objects")
        {
            this.move = move;
            this.movedObjects = movedObjects;
        }

        public override void Redo()
        {
            for (int i = 0; i < movedObjects.Count; i++)
            {
                movedObjects[i].Move(move[i]);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < movedObjects.Count; i++)
            {
                movedObjects[i].Move(move[-i]);
            }
        }
    }
}
