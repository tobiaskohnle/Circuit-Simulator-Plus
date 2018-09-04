using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulatorPlus
{
    public class ContextGate : Gate
    {
        /// <summary>
        /// The circuit inside of a gate.
        /// </summary>
        public List<Gate> Context { get; set; } = new List<Gate>();

        public override bool HasContext
        {
            get
            {
                return true;
            }
        }

        public override bool Eval()
        {
            throw new InvalidOperationException();
        }

        public override void CreateDefaultConnectionNodes()
        {
            throw new InvalidOperationException();
        }

        public void AddContext()
        {
            foreach (Gate gate in Context.ToList())
            {
                gate.Add();
            }
        }

        public void RemoveContext()
        {
            foreach (Gate gate in Context.ToList())
            {
                gate.Remove();
            }
        }
    }
}
