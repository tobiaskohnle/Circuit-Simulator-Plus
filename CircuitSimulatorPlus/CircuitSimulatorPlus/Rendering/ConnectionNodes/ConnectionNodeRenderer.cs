using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class ConnectionNodeRenderer
    {
        ConnectionNode connectionNode;
        Gate owner;
        bool isOutputNode;

        Ellipse invertionDot;
        Line connectionNodeLine;

        public ConnectionNodeRenderer(ConnectionNode connectionNode, Gate owner, bool isOutputNode)
        {
            this.connectionNode = connectionNode;
            this.owner = owner;
            this.isOutputNode = isOutputNode;

            connectionNodeLine = new Line
            {
                StrokeThickness = Constants.LineWidth
            };
            Panel.SetZIndex(connectionNodeLine, 1);

            invertionDot = new Ellipse
            {
                Width = Constants.InversionDotDiameter + Constants.LineWidth,
                Height = Constants.InversionDotDiameter + Constants.LineWidth,
                StrokeThickness = Constants.LineWidth
            };
            Panel.SetZIndex(invertionDot, 3);

            connectionNode.OnRenderedChanged += OnRenderedChanged;

            if (connectionNode.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (connectionNode.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(invertionDot);
                MainWindow.Self.canvas.Children.Add(connectionNodeLine);

                connectionNode.OnTickedChanged += UpdateLineStroke;
                connectionNode.OnSelectionChanged += UpdateLineStroke;
                connectionNode.OnStateChanged += UpdateLineStroke;
                connectionNode.OnInvertedChanged += OnInvertedChanged;
                connectionNode.OnNameChanged += OnNameChanged;
                connectionNode.OnCenteredChanged += OnPositionChanged;
                connectionNode.OnPositionChanged += OnPositionChanged;

                UpdateLineStroke();
                OnInvertedChanged();
                OnNameChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(connectionNodeLine);
                MainWindow.Self.canvas.Children.Remove(invertionDot);

                connectionNode.OnTickedChanged -= UpdateLineStroke;
                connectionNode.OnSelectionChanged -= UpdateLineStroke;
                connectionNode.OnStateChanged -= UpdateLineStroke;
                connectionNode.OnInvertedChanged -= OnInvertedChanged;
                connectionNode.OnNameChanged -= OnNameChanged;
                connectionNode.OnCenteredChanged -= OnPositionChanged;
                connectionNode.OnPositionChanged -= OnPositionChanged;
            }
        }

        void UpdateLineStroke()
        {
            bool state = connectionNode.IsInverted ? connectionNode.State == isOutputNode : connectionNode.State;
            if (connectionNode.IsSelected)
            {
                if (connectionNode.IsEmpty) // temp
                {
                    connectionNodeLine.Stroke = Brushes.Cyan; // temp
                }
                else
                {
                    connectionNodeLine.Stroke = MainWindow.Self.Theme.SelectedHighlight;
                }
            }
            else if (connectionNode.IsTicked)
            {
                connectionNodeLine.Stroke = MainWindow.Self.Theme.Ticked;
            }
            else if (!connectionNode.IsSelected)
            {
                connectionNodeLine.Stroke = state ? MainWindow.Self.Theme.High : MainWindow.Self.Theme.Low;
            }
            if (connectionNode.IsInverted)
            {
                invertionDot.Stroke = !state ? MainWindow.Self.Theme.High : MainWindow.Self.Theme.Low;
            }
        }

        public void OnMasterSlaveChanged()
        {
        }

        public void OnInvertedChanged()
        {
            invertionDot.Visibility = connectionNode.IsInverted ? Visibility.Visible : Visibility.Collapsed;
            OnPositionChanged();
        }

        public void OnNameChanged()
        {
        }

        public void OnPositionChanged()
        {
            var sX = isOutputNode ? 1 : -1;
            var y = connectionNode.Position.Y;

            if (connectionNode.IsCentered)
            {
                y = connectionNode.Owner.Position.Y + connectionNode.Owner.Size.Height / 2;
            }

            connectionNodeLine.X1 = connectionNode.Position.X;
            connectionNodeLine.Y1 = y;

            connectionNodeLine.X2 = connectionNode.Position.X + sX * Constants.ConnectionNodeLineLength;
            connectionNodeLine.Y2 = y;

            if (connectionNode.IsInverted)
            {
                connectionNodeLine.X1 = connectionNode.Position.X + sX * (Constants.InversionDotDiameter + Constants.LineRadius);
                Canvas.SetLeft(invertionDot, connectionNode.Position.X - invertionDot.Width / 2 + sX * invertionDot.Width / 2);
                Canvas.SetTop(invertionDot, connectionNode.Position.Y - invertionDot.Height / 2);
            }
        }
    }
}
