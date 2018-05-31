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
        List<Gate>createdGates;
        Gate gate;
        Gate.GateType gateType;
        Point position;

        public CreateGateAction(Gate gate, Gate.GateType gateType,Point position,List<Gate>createdGates,string message) : base(message)
        {
            this.gate = gate;
            this.gateType = gateType;
            this.position = position;
            this.createdGates = createdGates;
        }
        public override void Redo()
        {
            gate.Renderer.Render();
            createdGates.Add(gate);
        }

        public override void Undo()
        {
            gate.Renderer.Unrender();
            createdGates.Remove(gate);
        }
    }
}
