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

        public void Connect(OutputNode output, InputNode input)
        {
            output.ConnectTo(input);
        }

        public void Connect(InputNode input, OutputNode output)
        {
            Connect(output, input);
        }

        public void ClearNode(ConnectionNode nodeToClear)
        {
            nodeToClear.Clear();
        }
    }
}
