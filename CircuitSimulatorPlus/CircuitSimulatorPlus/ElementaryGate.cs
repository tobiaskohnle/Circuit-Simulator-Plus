using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public abstract class ElementaryGate
    {
        protected List<ElementaryConnection> connections;

        protected abstract bool Update();

        bool state;

        public List<ElementaryGate> Tick()
        {
            var tickedGates = new List<ElementaryGate>();
            if ((state = Update()) != state)
                foreach (ElementaryConnection connection in connections)
                {
                    tickedGates.Add(connection.Next);
                }
            return tickedGates;
        }
    }
}
