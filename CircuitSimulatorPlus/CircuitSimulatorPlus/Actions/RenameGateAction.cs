﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public class RenameGateAction : Action
    {
        Gate gate;
        string oldName;
        string newName;

        public RenameGateAction(Gate gate, string name) : base($"Renamed Gate to {name}")
        {
            this.gate = gate;
            newName = name;
            oldName = gate.Name;
        }

        public override void Redo()
        {
            gate.Name = oldName;
        }

        public override void Undo()
        {
            gate.Name = newName;
        }
    }
}
