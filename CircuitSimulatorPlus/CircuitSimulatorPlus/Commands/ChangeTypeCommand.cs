using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ChangeTypeCommand : Command
    {
        StorageObject storageObject = null;
        Type Type, oldtype;
        IList<IClickable> SelectedObjects;
        public ChangeTypeCommand(Type type, IList<IClickable> selectedObjects) : base($"Changed Type to {type.Name}")
        {
            Type = type;
            SelectedObjects = selectedObjects;
            oldtype = typeof(Gate);
        }

        public override void Redo()
        {
            foreach (IClickable obj in SelectedObjects.ToList())
            {
                if (obj is Gate)
                {
                    var gate = obj as Gate;
                    Gate newGate = null;

                    if (Type == typeof(ContextGate))
                        newGate = StorageConverter.ToGate(storageObject);
                    else if (Type == typeof(InputSwitch))
                        newGate = new InputSwitch();
                    else if (Type == typeof(OutputLight))
                        newGate = new OutputLight();
                    else if (Type == typeof(AndGate))
                        newGate = new AndGate();
                    else if (Type == typeof(OrGate))
                        newGate = new OrGate();
                    else if (Type == typeof(NopGate))
                        newGate = new NopGate();
                    else if (Type == typeof(SegmentDisplay))
                        newGate = new SegmentDisplay();

                    newGate.CopyFrom(gate);

                    MainWindow.Self.ContextGate.Context.Remove(gate);
                    MainWindow.Self.ContextGate.Context.Add(newGate);

                    foreach (OutputNode outputNode in newGate.Output)
                        MainWindow.Self.Tick(outputNode);

                    MainWindow.Self.Deselect(gate);
                    MainWindow.Self.Select(newGate);

                    //gate.IsRendered = false;
                    //newGate.IsRendered = true;

                    //MainWindow.Self.UpdateClickableObjects();
                }
            }
        }
        public override void Undo()
        {
            foreach (IClickable obj in SelectedObjects.ToList())
            {
                if (obj is Gate)
                {
                    var gate = obj as Gate;
                    Gate newGate = null;

                    if (oldtype == typeof(ContextGate))
                        newGate = StorageConverter.ToGate(storageObject);
                    else if (oldtype == typeof(InputSwitch))
                        newGate = new InputSwitch();
                    else if (oldtype == typeof(OutputLight))
                        newGate = new OutputLight();
                    else if (oldtype == typeof(AndGate))
                        newGate = new AndGate();
                    else if (oldtype == typeof(OrGate))
                        newGate = new OrGate();
                    else if (oldtype == typeof(NopGate))
                        newGate = new NopGate();
                    else if (oldtype == typeof(SegmentDisplay))
                        newGate = new SegmentDisplay();

                    newGate.CopyFrom(gate);

                    MainWindow.Self.ContextGate.Context.Remove(gate);
                    MainWindow.Self.ContextGate.Context.Add(newGate);

                    foreach (OutputNode outputNode in newGate.Output)
                        MainWindow.Self.Tick(outputNode);

                    MainWindow.Self.Deselect(gate);
                    MainWindow.Self.Select(newGate);

                    //gate.IsRendered = false;
                    //newGate.IsRendered = true;

                    //MainWindow.Self.UpdateClickableObjects();
                }
            }
        }
    }
}
