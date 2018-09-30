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

        public override void CopyFrom(Gate gate)
        {
            List<InputNode> input = Input;
            List<OutputNode> output = Output;

            base.CopyFrom(gate);

            minAmtInputNodes = input.Count;
            maxAmtInputNodes = input.Count;
            minAmtOutputNodes = output.Count;
            maxAmtOutputNodes = output.Count;
            UpdateAmtConnectionNodes();

            for (int i = 0; i < Input.Count; i++)
            {
                Input[i].CopyFrom(input[i]);
            }
            for (int i = 0; i < Output.Count; i++)
            {
                Output[i].CopyFrom(output[i], i);
            }
        }

        int minAmtInputNodes;
        int maxAmtInputNodes;
        int minAmtOutputNodes;
        int maxAmtOutputNodes;

        public override int MinAmtInputNodes
        {
            get
            {
                return minAmtInputNodes;
            }
        }
        public override int MaxAmtInputNodes
        {
            get
            {
                return maxAmtInputNodes;
            }
        }
        public override int MinAmtOutputNodes
        {
            get
            {
                return minAmtOutputNodes;
            }
        }
        public override int MaxAmtOutputNodes
        {
            get
            {
                return maxAmtOutputNodes;
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
            minAmtInputNodes = Input.Count;
            maxAmtInputNodes = Input.Count;
            minAmtOutputNodes = Output.Count;
            maxAmtOutputNodes = Output.Count;
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
