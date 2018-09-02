using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    class ToggleObjectsCommand : Command
    {
        List<IClickable> SelectedObjects;
        public ToggleObjectsCommand(List<IClickable> selectedObjects) : base("Toggle Objects")
        {
            SelectedObjects = selectedObjects;
        }

        public override void Redo()
        {
            foreach (IClickable obj in SelectedObjects)
            {
                if (obj is ConnectionNode)
                {
                    (obj as ConnectionNode).Invert();
                    MainWindow.Self.Tick(obj as ConnectionNode);
                }
                else if (obj is InputSwitch)
                {
                    (obj as InputSwitch).Toggle();
                    MainWindow.Self.Tick((obj as InputSwitch).Output[0]);
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
                    MainWindow.Self.Tick(obj as ConnectionNode);
                }
                else if (obj is InputSwitch)
                {
                    (obj as InputSwitch).Toggle();
                    MainWindow.Self.Tick((obj as InputSwitch).Output[0]);
                }
            }
        }
    }
}
