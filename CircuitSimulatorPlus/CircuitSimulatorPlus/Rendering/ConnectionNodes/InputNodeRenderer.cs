using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class InputNodeRenderer
    {
        InputNode inputNode;

        Line upperRisingEdgeLine;
        Line lowerRisingEdgeLine;

        public InputNodeRenderer(InputNode inputNode)
        {
            this.inputNode = inputNode;

            upperRisingEdgeLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };
            lowerRisingEdgeLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };

            inputNode.OnRenderedChanged += OnRenderedChanged;

            if (inputNode.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (inputNode.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(upperRisingEdgeLine);
                MainWindow.Self.canvas.Children.Add(lowerRisingEdgeLine);

                inputNode.OnRisingEdgeChanged += OnRisingEdgeChanged;
                inputNode.OnPositionChanged += OnPositionChanged;

                OnRisingEdgeChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(upperRisingEdgeLine);
                MainWindow.Self.canvas.Children.Remove(lowerRisingEdgeLine);

                inputNode.OnRisingEdgeChanged -= OnRisingEdgeChanged;
                inputNode.OnPositionChanged -= OnPositionChanged;
            }
        }

        public void OnRisingEdgeChanged()
        {
            Visibility lineVisibility = inputNode.IsRisingEdge ? Visibility.Visible : Visibility.Collapsed;
            upperRisingEdgeLine.Visibility = lowerRisingEdgeLine.Visibility = lineVisibility;
        }

        public void OnPositionChanged()
        {
            upperRisingEdgeLine.X1 = inputNode.Position.X;
            upperRisingEdgeLine.X2 = inputNode.Position.X + Constants.InversionDotDiameter * 1.2;
            upperRisingEdgeLine.Y1 = inputNode.Position.Y - Constants.InversionDotRadius * 1.2;
            upperRisingEdgeLine.Y2 = inputNode.Position.Y;

            lowerRisingEdgeLine.X1 = inputNode.Position.X;
            lowerRisingEdgeLine.X2 = inputNode.Position.X + Constants.InversionDotDiameter * 1.2;
            lowerRisingEdgeLine.Y1 = inputNode.Position.Y + Constants.InversionDotRadius * 1.2;
            lowerRisingEdgeLine.Y2 = inputNode.Position.Y;
        }
    }
}
