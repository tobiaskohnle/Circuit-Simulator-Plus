using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class OutputNodeRenderer
    {
        OutputNode outputNode;

        Line horzMasterSlaveLine;
        Line vertMasterSlaveLine;

        Label nameLabel;

        public OutputNodeRenderer(OutputNode outputNode)
        {
            this.outputNode = outputNode;

            horzMasterSlaveLine = new Line
            {
                Stroke = MainWindow.Self.Theme.MainColor,
                StrokeThickness = Constants.LineWidth
            };
            Panel.SetZIndex(horzMasterSlaveLine, 5);
            vertMasterSlaveLine = new Line
            {
                Stroke = MainWindow.Self.Theme.MainColor,
                StrokeThickness = Constants.LineWidth
            };
            Panel.SetZIndex(horzMasterSlaveLine, 5);

            nameLabel = new Label
            {
                Padding = new Thickness(0, 0, 0.2, 0),
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Width = outputNode.Owner.Size.Width,
                Foreground = MainWindow.Self.Theme.FontColor,
                Height = 1,
                FontSize = 0.5
            };
            Panel.SetZIndex(nameLabel, 5);

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

                MainWindow.Self.canvas.Children.Add(nameLabel);

                outputNode.OnMasterSlaveChanged += OnMasterSlaveChanged;
                outputNode.OnMasterSlaveChanged += OnPositionChanged;
                outputNode.OnNameChanged += OnNameChanged;
                outputNode.OnPositionChanged += OnPositionChanged;

                OnMasterSlaveChanged();
                OnNameChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(horzMasterSlaveLine);
                MainWindow.Self.canvas.Children.Remove(vertMasterSlaveLine);

                MainWindow.Self.canvas.Children.Remove(nameLabel);

                outputNode.OnMasterSlaveChanged -= OnMasterSlaveChanged;
                outputNode.OnMasterSlaveChanged -= OnPositionChanged;
                outputNode.OnNameChanged -= OnNameChanged;
                outputNode.OnPositionChanged -= OnPositionChanged;
            }
        }

        public void OnMasterSlaveChanged()
        {
            Visibility lineVisibility = outputNode.IsMasterSlave ? Visibility.Visible : Visibility.Collapsed;
            horzMasterSlaveLine.Visibility = vertMasterSlaveLine.Visibility = lineVisibility;
        }

        public void OnNameChanged()
        {
            nameLabel.Content = outputNode.Name;
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

            if (outputNode.IsMasterSlave)
            {
                Canvas.SetLeft(nameLabel, outputNode.Position.X - len - len - nameLabel.Width);
            }
            else
            {
                Canvas.SetLeft(nameLabel, outputNode.Position.X - nameLabel.Width);
            }
            Canvas.SetTop(nameLabel, outputNode.Position.Y - nameLabel.Height / 2);
        }
    }
}
