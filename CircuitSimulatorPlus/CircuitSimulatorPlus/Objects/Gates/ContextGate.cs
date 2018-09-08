using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class ContextGate : Gate
    {
        public List<Gate> Context { get; set; } = new List<Gate>();

        public ContextGate()
        {
            Size = new Size(3, 4);
        }

        public override int MinAmtInputNodes
        {
            get
            {
                return Input.Count;
            }
        }
        public override int MaxAmtInputNodes
        {
            get
            {
                return Input.Count;
            }
        }
        public override int MinAmtOutputNodes
        {
            get
            {
                return Output.Count;
            }
        }
        public override int MaxAmtOutputNodes
        {
            get
            {
                return Output.Count;
            }
        }

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
            //throw new InvalidOperationException();
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
