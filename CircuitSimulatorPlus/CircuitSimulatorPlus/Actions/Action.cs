using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public abstract class Action
    {
        protected string message;

        protected Action(string message)
        {
            this.message = message;
        }

        public abstract void Undo();
        public abstract void Redo();
    }
}
