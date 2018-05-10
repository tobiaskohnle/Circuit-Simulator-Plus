using System;
using System.Collections.Generic;

namespace CircuitSimulatorPlus
{
    public abstract class ElementaryGate
    {
        public List<ElementaryConnection> connections = new List<ElementaryConnection>();

        public List<bool> inputStates = new List<bool>();
        public bool lastState;

        public abstract bool Update();

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
            connections.Add(new ElementaryConnection(other, other.inputStates.Count));
        }

        public void DisconnectFrom(ElementaryGate other)
        {
            connections.RemoveAll(conn => conn.Next == other);
        }
    }
}
