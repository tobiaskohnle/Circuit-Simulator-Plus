using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus.Miscellaneous
{
    class GatePositionComparer : IComparer<Gate>
    {
        public static GatePositionComparer Instance = new GatePositionComparer();

        private GatePositionComparer() { }

        public int Compare(Gate x, Gate y)
        {
            int res = (int)(x.Position.Y - y.Position.Y);
            if (res == 0)
                res = (int)(x.Position.X - y.Position.X);
            return res;
        }
    }
}
