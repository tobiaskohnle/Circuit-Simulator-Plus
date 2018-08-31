using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class OutputNodeRenderer
    {
        OutputNode outputNode;

        Line horzMasterSlaveLine;
        Line vertMasterSlaveLine;

        public OutputNodeRenderer(OutputNode outputNode)
        {
            this.outputNode = outputNode;

            horzMasterSlaveLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };
            vertMasterSlaveLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };

            outputNode.OnRenderedChanged += OnRenderedChanged;

            if (outputNode.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (outputNode.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(horzMasterSlaveLine);
                MainWindow.Self.canvas.Children.Add(vertMasterSlaveLine);

                outputNode.OnMasterSlaveChanged += OnMasterSlaveChanged;
                outputNode.OnPositionChanged += OnPositionChanged;

                OnMasterSlaveChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(horzMasterSlaveLine);
                MainWindow.Self.canvas.Children.Remove(vertMasterSlaveLine);

                outputNode.OnMasterSlaveChanged -= OnMasterSlaveChanged;
                outputNode.OnPositionChanged -= OnPositionChanged;
            }
        }

        public void OnMasterSlaveChanged()
        {
            Visibility lineVisibility = outputNode.IsMasterSlave ? Visibility.Visible : Visibility.Collapsed;
            horzMasterSlaveLine.Visibility = vertMasterSlaveLine.Visibility = lineVisibility;
        }

        public void OnPositionChanged()
        {
            double len = 0.75;

            horzMasterSlaveLine.Y1 = horzMasterSlaveLine.Y2 = outputNode.Position.Y - len / 2;
            horzMasterSlaveLine.X1 = outputNode.Position.X - len;
            horzMasterSlaveLine.X2 = outputNode.Position.X - len - len;

            vertMasterSlaveLine.X1 = vertMasterSlaveLine.X2 = outputNode.Position.X - len;
            vertMasterSlaveLine.Y1 = outputNode.Position.Y - len / 2;
            vertMasterSlaveLine.Y2 = outputNode.Position.Y + len / 2;
        }
    }
}
