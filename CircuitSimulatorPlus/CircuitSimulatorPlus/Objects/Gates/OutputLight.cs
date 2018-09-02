﻿using CircuitSimulatorPlus.Rendering;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OutputLight : Gate
    {
        public OutputLight() : base(1, 0)
        {
            new OutputLightRenderer(this);
            Size = new Size(2, 2);
        }

        public bool State { get; internal set; }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }
    }
}
