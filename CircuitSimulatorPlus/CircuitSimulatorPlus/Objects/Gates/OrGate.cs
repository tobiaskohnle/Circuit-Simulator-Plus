using System.Windows;

namespace CircuitSimulatorPlus
{
    public class OrGate : Gate
    {
        public OrGate()
        {
            Size = new Size(3, 4);
            Tag = "\u22651";
        }

        public override int MinAmtInputNodes
        {
            get
            {
                return 2;
            }
        }
        public override int MinAmtOutputNodes
        {
            get
            {
                return 1;
            }
        }
        public override int MaxAmtOutputNodes
        {
            get
            {
                return 1;
            }
        }

        public override void CreateDefaultConnectionNodes()
        {
            CreateConnectionNodes(2, 1);
        }

        public override bool Eval()
        {
            foreach (InputNode input in Input)
                if (input.LogicState)
                    return true;
            return false;
        }
    }
}
