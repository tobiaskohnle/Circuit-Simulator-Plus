using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class CreateGateCommand : Command
    {
        Gate CreatedGate;
        public CreateGateCommand(Gate createdGate) : base($"created {createdGate.GetType()}")
        {
            this.CreatedGate = createdGate;
        }

        public override void Redo()
        {
            GateRenderer Temp = new GateRenderer(CreatedGate);
            CreatedGate.IsRendered = true;
            MainWindow.Self.ContextGate.Context.Add(CreatedGate);
            foreach (ConnectionNode node in CreatedGate.Input)
                node.IsRendered = true;
            foreach (ConnectionNode node in CreatedGate.Output)
                node.IsRendered = true;
            MainWindow.Self.Select(CreatedGate);
            MainWindow.Self.ClickableObjects.Add(CreatedGate);
            MainWindow.Self.ContextGate.Context.Add(CreatedGate);
        }

        public override void Undo()
        {
            CreatedGate.IsRendered = false;
            MainWindow.Self.ContextGate.Context.Remove(CreatedGate);

            foreach (ConnectionNode node in CreatedGate.Input)
                node.IsRendered = false;
            foreach (ConnectionNode node in CreatedGate.Output)
                node.IsRendered = false;
            MainWindow.Self.ClickableObjects.Remove(CreatedGate);
            MainWindow.Self.ContextGate.Context.Remove(CreatedGate);
        }
    }
}
