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
        Line upperRisingEdgeLine;
        Line lowerRisingEdgeLine;
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
                StrokeThickness = MainWindow.LineWidth
            };

            invertionDot = new Ellipse
            {
                Width = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                Height = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                StrokeThickness = MainWindow.LineWidth
            };

            connectionNode.OnTickedChanged += OnTickedChanged;
            connectionNode.OnSelectionChanged += OnSelectionChanged;
            connectionNode.OnStateChanged += OnStateChanged;
            if (connectionNode is InputNode)
                (connectionNode as InputNode).OnRisingEdgeChanged += OnRisingEdgeChanged;
            if (connectionNode is OutputNode)
                (connectionNode as OutputNode).OnMasterSlaveChanged += OnMasterSlaveChanged;
            connectionNode.OnInvertedChanged += OnInvertedChanged;
            connectionNode.OnNameChanged += OnNameChanged;
            connectionNode.OnPositionChanged += OnPositionChanged;
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

                OnTickedChanged();
                OnStateChanged();
                OnSelectionChanged();
                OnRisingEdgeChanged();
                OnMasterSlaveChanged();
                OnInvertedChanged();
                OnNameChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(connectionNodeLine);
                MainWindow.Self.canvas.Children.Remove(invertionDot);
                MainWindow.Self.canvas.Children.Remove(upperRisingEdgeLine);
                MainWindow.Self.canvas.Children.Remove(lowerRisingEdgeLine);
            }
        }

        void UpdateLineStroke()
        {
            //if (connectionNode.IsSelected)
            //{
            //    connectionNodeLine.Stroke = SystemColors.MenuHighlightBrush;
            //}
            //else if (connectionNode.IsTicked)
            //{
            //    connectionNodeLine.Stroke = Brushes.Orange;
            //}
            if (connectionNode.IsTicked)
            {
                connectionNodeLine.Stroke = Brushes.Orange;
            }
            else if (connectionNode.IsSelected)
            {
                connectionNodeLine.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                bool state = connectionNode.IsInverted ? connectionNode.State == isOutputNode : connectionNode.State;
                if (!connectionNode.IsSelected)
                {
                    connectionNodeLine.Stroke = state ? ActiveStateBrush : DefaultStateBrush;
                }
                if (connectionNode.IsInverted)
                {
                    invertionDot.Stroke = !state ? ActiveStateBrush : DefaultStateBrush;
                }
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

        public void OnRisingEdgeChanged()
        {
            if (isOutputNode == false)
            {
                if ((connectionNode as InputNode).IsRisingEdge)
                {
                    upperRisingEdgeLine = new Line
                    {
                        StrokeThickness = MainWindow.LineWidth,
                        X1 = connectionNode.Position.X
                            - connectionNode.AlignmentVector.Y * MainWindow.InversionDotRadius,
                        Y1 = connectionNode.Position.Y
                            + connectionNode.AlignmentVector.X * MainWindow.InversionDotRadius,
                        X2 = connectionNode.Position.X
                            - connectionNode.AlignmentVector.X * MainWindow.InversionDotDiameter,
                        Y2 = connectionNode.Position.Y
                            - connectionNode.AlignmentVector.Y * MainWindow.InversionDotDiameter
                    };
                    lowerRisingEdgeLine = new Line
                    {
                        StrokeThickness = MainWindow.LineWidth,
                        X1 = connectionNode.Position.X
                            + connectionNode.AlignmentVector.Y * MainWindow.InversionDotRadius,
                        Y1 = connectionNode.Position.Y
                            - connectionNode.AlignmentVector.X * MainWindow.InversionDotRadius,
                        X2 = connectionNode.Position.X
                            - connectionNode.AlignmentVector.X * MainWindow.InversionDotDiameter,
                        Y2 = connectionNode.Position.Y
                            - connectionNode.AlignmentVector.Y * MainWindow.InversionDotDiameter
                    };

                    MainWindow.Self.canvas.Children.Add(upperRisingEdgeLine);
                    MainWindow.Self.canvas.Children.Add(lowerRisingEdgeLine);
                }
                else
                {
                    MainWindow.Self.canvas.Children.Remove(upperRisingEdgeLine);
                    MainWindow.Self.canvas.Children.Remove(lowerRisingEdgeLine);
                }
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
            connectionNodeLine.X1 = connectionNode.Position.X;
            connectionNodeLine.Y1 = connectionNode.Position.Y;

            connectionNodeLine.X2 = connectionNode.Position.X
                + connectionNode.AlignmentVector.X * MainWindow.ConnectionNodeLineLength;
            connectionNodeLine.Y2 = connectionNode.Position.Y
                + connectionNode.AlignmentVector.Y * MainWindow.ConnectionNodeLineLength;

            if (connectionNode.IsInverted)
            {
                connectionNodeLine.X1 = connectionNode.Position.X
                    + connectionNode.AlignmentVector.X * (MainWindow.InversionDotDiameter + MainWindow.LineRadius);
                Canvas.SetLeft(invertionDot, connectionNode.Position.X
                    - invertionDot.Width / 2
                    + connectionNode.AlignmentVector.X * invertionDot.Width / 2);
                Canvas.SetTop(invertionDot, connectionNode.Position.Y
                    - invertionDot.Height / 2
                    + connectionNode.AlignmentVector.Y * invertionDot.Height / 2);
            }
        }
    }
}
