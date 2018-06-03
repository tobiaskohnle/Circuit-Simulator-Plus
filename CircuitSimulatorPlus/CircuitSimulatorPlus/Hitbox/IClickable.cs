using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public interface IClickable
    {
        Hitbox Hitbox { get; set; }
        bool IsSelected { get; set; }
        bool IsMovable { get; }
    }
}
