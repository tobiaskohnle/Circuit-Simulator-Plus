using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public abstract class Command
    {
        protected string message;

        protected Command(string message)
        {
            this.message = message;
        }

        public abstract void Undo();
        public abstract void Redo();
    }
}
