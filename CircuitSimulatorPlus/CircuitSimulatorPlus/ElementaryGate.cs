using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ElementaryGate
    {
        protected List<ElementaryConnection> connections;

        protected List<bool> inputStates;

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

        public void ConnectTo(ElementaryGate other)
        {
            connections.Add(new ElementaryConnection(other, connections.Count));
        }

        public void DisconnectFrom(ElementaryGate other)
        {
            connections.RemoveAll(conn => conn.Next == other);
        }
    }
}
