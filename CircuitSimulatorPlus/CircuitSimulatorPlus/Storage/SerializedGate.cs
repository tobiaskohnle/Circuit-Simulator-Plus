using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class SerializedGate
    {
        public string Name;
        public string Tag;
        public Point Position;
        public Size Size;
        public string Type;
        public List<SerializedGate> Context;
        public int[] InputConnections;
        public int[] OutputConnections;
        public List<int> InvertedInputs;
        public List<int> InvertedOutputs;
        public List<int> InitialActiveOutputs;
        public List<int> RisingEdgeInputs;
        public List<int> MasterSlaveOutputs;
        public List<int> CenteredInputs;
        public List<int> CenteredOutputs;
        public int[] CableEndPoints;
        public List<SerializedGate.Cable> Cables;
        public string[] InputLabels;
        public string[] OutputLabels;

        public class Cable
        {
            public int OutputConnection;
            public int EndPoint;
            public List<Point> Points;
        }
    }
}
