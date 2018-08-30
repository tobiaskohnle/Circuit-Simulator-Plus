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
            //horzMasterSlaveLine.X1 = 
        }
    }
}
