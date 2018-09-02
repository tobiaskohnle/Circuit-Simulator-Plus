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
        public string Name
        {
            get; set;
        }
        public Point Position
        {
            get; set;
        }
        public string Type
        {
            get; set;
        }
        public List<StorageObject> Context
        {
            get; set;
        }
        public int[] InputConnections
        {
            get; set;
        }
        public int[] OutputConnections
        {
            get; set;
        }
        public List<int> InvertedInputs
        {
            get; set;
        }
        public List<int> InvertedOutputs
        {
            get; set;
        }
        public List<int> InitialActiveOutputs
        {
            get; set;
        }
    }
}
