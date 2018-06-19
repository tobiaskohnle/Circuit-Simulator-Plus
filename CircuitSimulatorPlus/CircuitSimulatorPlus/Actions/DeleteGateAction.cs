using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class DeleteGateAction : Action
    {
        List<Gate> deletedGates;
        Gate contextGate;

        public DeleteGateAction(Gate contextGate, List<Gate> deletedGates) : base($"Deleted Gates")
        {
            this.deletedGates = deletedGates;
            this.contextGate = contextGate;
        }

        public override void Redo()
        {
            foreach (Gate gate in deletedGates)
                gate.Renderer.Render();
            //contextGate.Context.AddRange(deletedGates);
        }

        public override void Undo()
        {
            foreach (Gate gate in deletedGates)
                gate.Renderer.Unrender();
            //contextGate.Context = contextGate.Context.Except(deletedGates).ToList();
        }
    }
}
