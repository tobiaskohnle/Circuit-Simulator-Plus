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
        List<Line> connectionLines;
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

            Render();

            OnStateChanged();
            OnSelectionChanged();
            OnRisingEdgeChanged();
            OnMasterSlaveChanged();
            OnNameChanged();
            OnInvertedChanged();
            OnPositionChanged();
        }

        public void Render()
        {
            connectionNodeLine = new Line
            {
                StrokeThickness = MainWindow.LineWidth
            };
            connectionLines = new List<Line>();

            canvas.Children.Add(connectionNodeLine);
            foreach (Line connectionLine in connectionLines)
                canvas.Children.Add(connectionLine);

            OnPositionChanged();
            OnInvertedChanged();
            OnStateChanged();
            OnConnectedNodesChanged();
        }

        public void Unrender()
        {
            canvas.Children.Remove(connectionNodeLine);
            foreach (Line connectionLine in connectionLines)
                canvas.Children.Remove(connectionLine);
            if (connectionNode.IsInverted)
                canvas.Children.Remove(invertionDot);
            if (isOutputNode == false && (connectionNode as InputNode).IsRisingEdge)
            {
                canvas.Children.Remove(upperRisingEdgeLine);
                canvas.Children.Remove(lowerRisingEdgeLine);
            }
        }

        public void OnConnectedNodesChanged()
        {
            connectionLines.Clear();

            if (connectionNode is OutputNode)
                foreach (ConnectionNode node in connectionNode.NextConnectedTo)
                {
                    connectionLines.Add(new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = MainWindow.LineWidth,
                        X1 = connectionNode.Position.X,
                        Y1 = connectionNode.Position.Y,
                        X2 = node.Position.X,
                        Y2 = node.Position.Y,
                    });
                }
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
                        X1 = connectionNode.Position.X,
                        Y1 = connectionNode.Position.Y - MainWindow.InversionDotRadius,
                        X2 = connectionNode.Position.X + MainWindow.InversionDotDiameter,
                        Y2 = connectionNode.Position.Y,
                    };
                    lowerRisingEdgeLine = new Line
                    {
                        StrokeThickness = MainWindow.LineWidth,
                        X1 = connectionNode.Position.X,
                        Y1 = connectionNode.Position.Y + MainWindow.InversionDotRadius,
                        X2 = connectionNode.Position.X + MainWindow.InversionDotDiameter,
                        Y2 = connectionNode.Position.Y,
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
            if (connectionNode.IsInverted)
            {
                invertionDot = new Ellipse
                {
                    Width = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                    Height = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                    StrokeThickness = MainWindow.LineWidth
                };
                canvas.Children.Add(invertionDot);
            }
            else
            {
                canvas.Children.Remove(invertionDot);
            }
            OnPositionChanged();
        }

        public void OnNameChanged()
        {
        }

        public void OnPositionChanged()
        {
            connectionNodeLine.Y1 = connectionNodeLine.Y2 = connectionNode.Position.Y;

            if (connectionNode.IsInverted)
            {
                if (isOutputNode)
                {
                    connectionNodeLine.X1 = connectionNode.Position.X + MainWindow.InversionDotDiameter + MainWindow.LineRadius;
                    if (connectionNode.IsInverted)
                        Canvas.SetLeft(invertionDot, connectionNode.Position.X);
                }
                else
                {
                    connectionNodeLine.X1 = connectionNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineRadius;
                    if (connectionNode.IsInverted)
                        Canvas.SetLeft(invertionDot, connectionNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineWidth);
                }
                if (connectionNode.IsInverted)
                    Canvas.SetTop(invertionDot, connectionNode.Position.Y - MainWindow.InversionDotRadius - MainWindow.LineRadius);
            }
            else
            {
                connectionNodeLine.X1 = connectionNode.Position.X;
            }
            if (isOutputNode)
            {
                connectionNodeLine.X2 = connectionNode.Position.X + MainWindow.Unit;
            }
            else
            {
                connectionNodeLine.X2 = connectionNode.Position.X - MainWindow.Unit;
            }
        }
    }
}
