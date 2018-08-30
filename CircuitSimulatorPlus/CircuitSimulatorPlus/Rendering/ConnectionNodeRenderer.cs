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
        Label nameLabel;

        public readonly Brush ActiveStateBrush = Brushes.Red;
        public readonly Brush DefaultStateBrush = Brushes.Black;

        public ConnectionNodeRenderer(ConnectionNode connectionNode, Gate owner, bool isOutputNode)
        {
            this.connectionNode = connectionNode;
            this.owner = owner;
            this.isOutputNode = isOutputNode;

            connectionNodeLine = new Line
            {
                StrokeThickness = Constants.LineWidth
            };

            invertionDot = new Ellipse
            {
                Width = Constants.InversionDotDiameter + Constants.LineWidth,
                Height = Constants.InversionDotDiameter + Constants.LineWidth,
                StrokeThickness = Constants.LineWidth
            };

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

                connectionNode.OnTickedChanged += OnTickedChanged;
                connectionNode.OnSelectionChanged += OnSelectionChanged;
                connectionNode.OnStateChanged += OnStateChanged;
                connectionNode.OnInvertedChanged += OnInvertedChanged;
                connectionNode.OnNameChanged += OnNameChanged;
                connectionNode.OnPositionChanged += OnPositionChanged;

                OnTickedChanged();
                OnStateChanged();
                OnSelectionChanged();
                OnInvertedChanged();
                OnNameChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(connectionNodeLine);
                MainWindow.Self.canvas.Children.Remove(invertionDot);

                connectionNode.OnTickedChanged -= OnTickedChanged;
                connectionNode.OnSelectionChanged -= OnSelectionChanged;
                connectionNode.OnStateChanged -= OnStateChanged;
                connectionNode.OnInvertedChanged -= OnInvertedChanged;
                connectionNode.OnNameChanged -= OnNameChanged;
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
                    connectionNodeLine.Stroke = SystemColors.MenuHighlightBrush;
                }
            }
            else if (connectionNode.IsTicked)
            {
                connectionNodeLine.Stroke = Brushes.Orange;
            }
            else if (!connectionNode.IsSelected)
            {
                connectionNodeLine.Stroke = state ? ActiveStateBrush : DefaultStateBrush;
            }
            if (connectionNode.IsInverted)
            {
                invertionDot.Stroke = !state ? ActiveStateBrush : DefaultStateBrush;
            }
        }

        public void OnTickedChanged()
        {
            UpdateLineStroke();
        }

        public void OnStateChanged()
        {
            UpdateLineStroke();
        }

        public void OnSelectionChanged()
        {
            UpdateLineStroke();
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
            var x = isOutputNode ? 1 : -1;

            connectionNodeLine.X1 = connectionNode.Position.X;
            connectionNodeLine.Y1 = connectionNode.Position.Y;

            connectionNodeLine.X2 = connectionNode.Position.X + x * Constants.ConnectionNodeLineLength;
            connectionNodeLine.Y2 = connectionNode.Position.Y;

            if (connectionNode.IsInverted)
            {
                connectionNodeLine.X1 = connectionNode.Position.X
                    + x * (Constants.InversionDotDiameter + Constants.LineRadius);
                Canvas.SetLeft(invertionDot, connectionNode.Position.X
                    - invertionDot.Width / 2
                    + x * invertionDot.Width / 2);
                Canvas.SetTop(invertionDot, connectionNode.Position.Y
                    - invertionDot.Height / 2
                    + x * invertionDot.Height / 2);
            }
        }
    }
}
