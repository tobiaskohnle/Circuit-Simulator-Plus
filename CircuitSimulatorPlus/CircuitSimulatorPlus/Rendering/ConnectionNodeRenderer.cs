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
        Line connectionLine;

        public readonly Brush ActiveStateBrush = Brushes.Red;
        public readonly Brush DefaultStateBrush = Brushes.Black;

        public ConnectionNodeRenderer(Canvas canvas, ConnectionNode connectionNode, Gate owner, bool isOutputNode)
        {
            this.canvas = canvas;
            this.connectionNode = connectionNode;
            this.owner = owner;
            this.isOutputNode = isOutputNode;

            Render();
        }

        public void Render()
        {
            connectionLine = new Line
            {
                StrokeThickness = MainWindow.LineWidth
            };
            invertionDot = new Ellipse
            {
                Width = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                Height = MainWindow.InversionDotDiameter + MainWindow.LineWidth,
                StrokeThickness = MainWindow.LineWidth
            };

            canvas.Children.Add(connectionLine);
            canvas.Children.Add(invertionDot);

            OnLayoutChanged();
            OnStateChanged();
        }

        public void Unrender()
        {
            canvas.Children.Remove(connectionLine);
            canvas.Children.Remove(invertionDot);
        }

        public void OnStateChanged()
        {
            bool state = connectionNode.IsInverted ? connectionNode.State == isOutputNode : connectionNode.State;

            if (connectionNode.IsSelected)
            {
                connectionLine.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                connectionLine.Stroke = state ? ActiveStateBrush : DefaultStateBrush;
            }
            invertionDot.Stroke = !state ? ActiveStateBrush : DefaultStateBrush;
        }

        public void OnLayoutChanged()
        {
            connectionLine.Y1 = connectionLine.Y2 = connectionNode.Position.Y;

            if (connectionNode.IsInverted)
            {
                if (isOutputNode)
                {
                    connectionLine.X1 = connectionNode.Position.X + MainWindow.InversionDotDiameter + MainWindow.LineRadius;
                }
                else
                {
                    connectionLine.X1 = connectionNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineRadius;
                }
                Canvas.SetTop(invertionDot, connectionNode.Position.Y - MainWindow.InversionDotRadius - MainWindow.LineRadius);
                if (isOutputNode)
                {
                    Canvas.SetLeft(invertionDot, connectionNode.Position.X);
                }
                else
                {
                    Canvas.SetLeft(invertionDot, connectionNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineWidth);
                }
            }
            else
            {
                connectionLine.X1 = connectionNode.Position.X;
            }
            if (isOutputNode)
            {
                connectionLine.X2 = connectionNode.Position.X + MainWindow.Unit;
            }
            else
            {
                connectionLine.X2 = connectionNode.Position.X - MainWindow.Unit;
            }

            if (!isOutputNode && ((InputNode)connectionNode).IsRisingEdge)
            {
                // TODO: rising rdge symbol
            }

            invertionDot.Visibility = connectionNode.IsInverted ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
