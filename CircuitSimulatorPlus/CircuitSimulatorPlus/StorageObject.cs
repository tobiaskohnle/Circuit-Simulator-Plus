using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class StorageObject
    {
        public string Name;
        public Point Position;
        public List<StorageObject> Context;
        public Gate.GateType Type;
        public int[] OutputConnections;
        public int[] InputConnections;
        public List<int> InvertedInputs;
        public List<int> InvertedOutputs;
        public List<int> InitialActiveOutputs;
    }
}
