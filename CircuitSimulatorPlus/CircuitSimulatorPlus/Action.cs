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

        protected abstract void Undo();
        protected abstract void Redo();

        protected Action(string message)
        {
            this.message = message;
        }
    }
}
