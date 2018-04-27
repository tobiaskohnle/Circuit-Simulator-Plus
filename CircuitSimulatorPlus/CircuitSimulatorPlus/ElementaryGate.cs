using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ElementaryGate
    {
        protected List<ElementaryConnection> connections;

        protected abstract bool Update();

        protected List<bool> state;

        bool lastState;

        public List<ElementaryGate> Tick()
        {
            var tickedGates = new List<ElementaryGate>();
            if ((lastState = Update()) != lastState)
                foreach (ElementaryConnection connection in connections)
                {
                    tickedGates.Add(connection.Next);
                }
            return tickedGates;
        }
    }
}
