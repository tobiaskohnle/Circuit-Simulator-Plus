using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class IndexEventArgs : EventArgs
    {
        public int Index;

        public IndexEventArgs(int index)
        {
            Index = index;
        }
    }
}
