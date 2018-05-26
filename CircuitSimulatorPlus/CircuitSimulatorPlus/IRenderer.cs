﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public interface IRenderer
    {
        void Render();
        void Unrender();
    }
}