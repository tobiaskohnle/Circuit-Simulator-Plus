using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class ChangeTypeCommand : Command
    {
        Type Oldtype;
        Type Newtype;
        public ChangeTypeCommand(Type oldtype, Type newtype) : base($"Changed Type to {newtype.Name}")
        {
            this.Oldtype = oldtype;
            this.Newtype = newtype;
        }

        public override void Redo()
        {
            MainWindow.Self.ChangeType(Newtype);
        }

        public override void Undo()
        {
            MainWindow.Self.ChangeType(Oldtype);
        }
    }
}
