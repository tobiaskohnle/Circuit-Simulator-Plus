using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class SerializedGate
    {
        public string Name;
        public Point Position;
        public string Type;
        public List<SerializedGate> Context;
        public int[] InputConnections;
        public int[] OutputConnections;
        public List<int> InvertedInputs;
        public List<int> InvertedOutputs;
        public List<int> InitialActiveOutputs;
        public List<int> RisingEdgeInputs;
        public List<int> MasterSlaveOutputs;
        public int[] CableEndPoints;
        public List<SerializedGate.Cable> Cables;
        
        public class Cable
        {
            public int OutputConnection;
            public int EndPoint;
            public List<Point> Points;
        }
    }
}
