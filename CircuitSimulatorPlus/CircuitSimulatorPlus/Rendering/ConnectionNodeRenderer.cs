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
        Canvas canvas;
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

        public ConnectionNodeRenderer(Canvas canvas, ConnectionNode connectionNode, Gate owner, bool isOutputNode)
        {
            this.canvas = canvas;
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

            Render();
        }

        public void Render()
        {
            canvas.Children.Add(invertionDot);
            canvas.Children.Add(connectionNodeLine);

            OnStateChanged();
            OnSelectionChanged();
            OnRisingEdgeChanged();
            OnMasterSlaveChanged();
            OnInvertedChanged();
            OnNameChanged();
            OnPositionChanged();
        }

        public void Unrender()
        {
            canvas.Children.Remove(connectionNodeLine);
            canvas.Children.Remove(invertionDot);
            canvas.Children.Remove(upperRisingEdgeLine);
            canvas.Children.Remove(lowerRisingEdgeLine);
        }

        public void OnStateChanged()
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

        public void OnSelectionChanged()
        {
            if (connectionNode.IsSelected)
            {
                connectionNodeLine.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                OnStateChanged();
            }
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

                    canvas.Children.Add(upperRisingEdgeLine);
                    canvas.Children.Add(lowerRisingEdgeLine);
                }
                else
                {
                    canvas.Children.Remove(upperRisingEdgeLine);
                    canvas.Children.Remove(lowerRisingEdgeLine);
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
