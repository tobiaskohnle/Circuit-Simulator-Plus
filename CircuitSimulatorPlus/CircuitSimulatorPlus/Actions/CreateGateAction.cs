using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CreateGateAction : Action
    {
        Gate createdGate;
        Gate contextGate;

        public CreateGateAction(Gate contextGate, Gate createdGate) : base("...")
        {
            this.createdGate = createdGate;
            this.contextGate = contextGate;
        }

        public override void Redo()
        {
            //createdGate.Renderer.Render();
            //contextGate.Context.Add(createdGate);
        }

        public override void Undo()
        {
            //createdGate.Renderer.Unrender();
            //contextGate.Context.Remove(createdGate);
        }
    }
}
