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
        Gate gate;
        Gate contextGate;

        public CreateGateAction(Gate contextGate, Gate gate) : base($"Created {gate.Type} Gate")
        {
            this.gate = gate;
            this.contextGate = contextGate;
        }

        public override void Redo()
        {
            gate.Renderer.Render();
            contextGate.Context.Add(gate);
        }

        public override void Undo()
        {
            gate.Renderer.Unrender();
            contextGate.Context.Remove(gate);
        }
    }
}
