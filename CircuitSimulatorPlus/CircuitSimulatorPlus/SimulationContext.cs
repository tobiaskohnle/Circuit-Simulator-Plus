using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class SimulationContext
    {
        List<ElementaryGate> elementaryGates = new List<ElementaryGate>();
        List<Gate> gates = new List<Gate>();

        public void RenderAllGates()
        {
            foreach (Gate gate in gates)
                gate.Renderer.Render();
        }

        public void Add(Gate gate)
        {
            gates.Add(gate);
            gate.Renderer.Render();
        }

        public void Remove(Gate gate)
        {
            gate.Renderer.Unrender();
            gates.Remove(gate);
        }

        public void Connect(Gate outputGate, int outputIndex, Gate inputGate, int inputIndex)
        {

        }

        public void Disconnect(Gate outputGate, int outputIndex, Gate inputGate, int inputIndex)
        {

        }
    }
}
