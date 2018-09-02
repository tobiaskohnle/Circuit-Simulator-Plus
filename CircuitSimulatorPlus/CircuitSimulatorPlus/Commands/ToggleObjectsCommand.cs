using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus.Actions
{
    class ToggleObjectsCommand : Command
    {
        List<IClickable> SelectedObjects;
        public ToggleObjectsCommand(List<IClickable> selectedObjects) : base("Toggle Objects")
        {
            this.SelectedObjects = selectedObjects;
        }

        public override void Redo()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Invert();
                }
                else if (obj is InputSwitch)
                {
                    (obj as InputSwitch).Toggle();
                }
            }
        }

        public override void Undo()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Invert();
                }
                else if (obj is InputSwitch)
                {
                    (obj as InputSwitch).Toggle();
                }
            }
        }
    }
}
