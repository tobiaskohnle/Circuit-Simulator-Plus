using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class DeleteCommand : Command
    {
        IList<IClickable> SelectedObjects;
        IList<IClickable> DeletedObjects;
        public DeleteCommand(IList<IClickable> selectedObjects) : base("Deleted object")
        {
            this.SelectedObjects = selectedObjects;
        }

        public override void Redo()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is Gate)
                {
                    DeletedObjects.Add(obj as Gate);
                    MainWindow.Self.Remove(obj as Gate);
                }
                else if (obj is ConnectionNode)
                {
                    DeletedObjects.Add(obj as ConnectionNode);
                    (obj as ConnectionNode).Clear();
                }
            }
        }

        public override void Undo()
        {
            foreach (IClickable obj in DeletedObjects)
            {
                if (obj is Gate)
                {
                    MainWindow.Self.Add(obj as Gate);
                    DeletedObjects.Remove(obj as Gate);
                }
                else if (obj is ConnectionNode)
                {
                    if (obj is InputNode)
                    {
                        (obj as InputNode).ConnectTo((obj as InputNode).BackConnectedTo);
                    }
                    else if (obj is OutputNode)
                    {

                    }
                }
            }
        }
    }
}
