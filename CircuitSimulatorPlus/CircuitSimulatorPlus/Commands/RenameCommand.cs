using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class RenameCommand : Command
    {
        List<IClickable> selectedObjects;
        string oldName;
        string newName;

        public RenameCommand(List<IClickable> selectedObjects, string name) : base($"Renamed to {name}")
        {
            this.selectedObjects = selectedObjects;
            newName = name;
        }

        public override void Redo()
        {
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is Gate)
                {
                    oldName = (obj as Gate).Name;
                    (obj as Gate).Name = newName;
                }
                else if (obj is ConnectionNode)
                {
                    oldName = (obj as ConnectionNode).Name;
                    (obj as ConnectionNode).Name = newName;
                }
            }
        }

        public override void Undo()
        {
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is Gate)
                {
                    (obj as Gate).Name = oldName;
                }
                else if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Name = oldName;
                }
            }
        }
    }
}
