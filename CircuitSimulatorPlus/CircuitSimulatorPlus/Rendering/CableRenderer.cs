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
        Cable cable;
        Line cableLine;

        public CableRenderer(Canvas canvas, Cable cable)
        {
            this.canvas = canvas;
            this.cable = cable;
            cable.InputNode.CableRenderer = this;
            cable.OutputNode.CableRenderer = this;

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
            cableLine.X1 = cable.InputNode.Position.X;
            cableLine.Y1 = cable.InputNode.Position.Y;
            cableLine.X2 = cable.OutputNode.Position.X;
            cableLine.Y2 = cable.OutputNode.Position.Y;
        }
    }
}
