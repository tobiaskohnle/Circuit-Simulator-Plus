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
        int xDir;

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
            xDir = isOutputNode ? 1 : -1;

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
                Width = MainWindow.InversionDotDiameter,
                Height = MainWindow.InversionDotDiameter,
                StrokeThickness = MainWindow.LineWidth
            };

            canvas.Children.Add(connectionLine);
            canvas.Children.Add(invertionDot);
        }

        public void Unrender()
        {
            canvas.Children.Remove(connectionLine);
            canvas.Children.Remove(invertionDot);
        }

        public void OnStateChanged()
        {
            invertionDot.Stroke = connectionNode.State ? ActiveStateBrush : DefaultStateBrush;
            connectionLine.Stroke = connectionNode.State ? DefaultStateBrush : ActiveStateBrush;
        }

        public void OnLayoutChanged()
        {
            connectionLine.Y1 = connectionLine.Y2 = connectionNode.Position.Y;

            if (connectionNode.IsInverted)
            {
                connectionLine.X1 = connectionNode.Position.X + (MainWindow.InversionDotDiameter + MainWindow.LineRadius) * xDir;
                Canvas.SetTop(invertionDot, 0);
                Canvas.SetLeft(invertionDot, 0);
            }
            else
            {
                connectionLine.X1 = connectionNode.Position.X + MainWindow.InversionDotRadius * (1 + xDir);
            }
            connectionLine.X2 = connectionNode.Position.X + MainWindow.Unit * xDir;

            if (!isOutputNode)
            {

            }

            invertionDot.Visibility = connectionNode.IsInverted ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
