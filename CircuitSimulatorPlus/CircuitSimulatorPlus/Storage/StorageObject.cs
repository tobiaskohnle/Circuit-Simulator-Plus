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
        public string Type;
        public List<StorageObject> Context;
        public int[] InnerInputConnections;
        public int[] InnerOutputConnections;
        public int[] InputConnections;
        public int[] OutputConnections;
        public List<int> InvertedInputs;
        public List<int> InvertedOutputs;
        public List<int> InitialActiveOutputs;
    }
}
