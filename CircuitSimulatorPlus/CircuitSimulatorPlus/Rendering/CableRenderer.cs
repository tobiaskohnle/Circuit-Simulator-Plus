using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class CableRenderer
    {
        Canvas canvas;
        ConnectionNode inputNode;
        ConnectionNode outputNode;
        Line cableLine;

        public CableRenderer(Canvas canvas, ConnectionNode inputNode, ConnectionNode outputNode)
        {
            this.canvas = canvas;
            this.inputNode = inputNode;
            this.outputNode = outputNode;
            inputNode.CableRenderer = this;
            outputNode.CableRenderer = this;

            cableLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = MainWindow.LineWidth
            };

            Render();
        }

        public void Render()
        {
            canvas.Children.Add(cableLine);

            OnLayoutChanged();
        }

        public void Unrender()
        {
            canvas.Children.Remove(cableLine);
        }

        public void OnLayoutChanged()
        {
            cableLine.X1 = inputNode.Position.X;
            cableLine.Y1 = inputNode.Position.Y;
            cableLine.X2 = outputNode.Position.X;
            cableLine.Y2 = outputNode.Position.Y;
        }
    }
}
