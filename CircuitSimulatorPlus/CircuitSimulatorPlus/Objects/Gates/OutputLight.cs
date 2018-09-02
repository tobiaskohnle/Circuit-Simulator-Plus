using CircuitSimulatorPlus.Miscellaneous;
using CircuitSimulatorPlus.Rendering;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CircuitSimulatorPlus
{
    public class OutputLight : Gate
    {
        public OutputLight(ContextGate parent) : this()
        {
            Parent = parent;
            OnPositionChanged += SortOutputs;
        }

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
        private ContextGate parent;
        private object indexText;

        public ContextGate Parent
        
        {
            set
            {
                if (parent != null)
                {
                    parent.OutputLights.Remove(this);
                }
                parent = value;
                if (parent != null)
                {
                    parent.OutputLights.Add(this);
                    SortOutputs();
                }
            }
            get { return parent; }
        }

        public int Index
        {
            get
            {
                if (Parent == null)
                    throw new Exception("OutputLight has no parent");
                return Parent.OutputLights.BinarySearch(this, GatePositionComparer.Instance);
            }
        }
       
        private void SortOutputs()
        {
            Parent.OutputLights.Sort(GatePositionComparer.Instance);
        }
    }
}
