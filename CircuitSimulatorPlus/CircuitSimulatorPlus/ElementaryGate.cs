using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ElementaryGate
    {
        protected List<ElementaryConnection> connections;

        protected List<bool> state;

        protected bool lastState;

        protected abstract bool Update();

        public List<ElementaryGate> Tick()
        {
            var tickedGates = new List<ElementaryGate>();
            if (lastState != (lastState = Update()))
                foreach (ElementaryConnection connection in connections)
                    tickedGates.Add(connection.Next);
            return tickedGates;
        }
    }
}
