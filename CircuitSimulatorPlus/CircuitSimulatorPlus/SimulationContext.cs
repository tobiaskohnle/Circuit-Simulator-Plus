using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class SimulationContext
    {
        List<Gate> gates = new List<Gate>();

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
    }
}
