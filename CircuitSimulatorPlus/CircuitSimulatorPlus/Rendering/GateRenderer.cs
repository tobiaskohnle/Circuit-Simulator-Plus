using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    /// <summary>
    /// Documentation is elementary.
    /// </summary>
    public class GateRenderer
    {
        Canvas canvas;
        Gate gate;

        Rectangle boundingBox;
        Label tagLabel;
        Label nameLabel;

        List<ConnectionNodeRenderer> connectionNodeRenderers;

        public GateRenderer(Canvas canvas, Gate gate)
        {
            this.canvas = canvas;
            this.gate = gate;

            Render();
        }

        public void Render()
        {
            boundingBox = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = MainWindow.LineWidth,
                Width = gate.Size.Width + MainWindow.LineWidth,
                Height = gate.Size.Height + MainWindow.LineWidth
            };

            nameLabel = new Label
            {
                Width = gate.Size.Width,
                Height = MainWindow.Unit,
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = MainWindow.Unit / 2
            };

            tagLabel = new Label
            {
                Width = gate.Size.Width,
                Height = gate.Size.Height,
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = MainWindow.Unit,
                Content = gate.Tag
            };

            canvas.Children.Add(boundingBox);
            canvas.Children.Add(nameLabel);
            canvas.Children.Add(tagLabel);
        }

        public void Unrender()
        {
            canvas.Children.Remove(boundingBox);
            canvas.Children.Remove(nameLabel);
            canvas.Children.Remove(tagLabel);

            foreach (ConnectionNodeRenderer renderer in connectionNodeRenderers)
            {
                renderer.Unrender();
            }
        }

        public void OnLayoutChanged()
        {
            Canvas.SetLeft(boundingBox, gate.Position.X - MainWindow.LineRadius);
            Canvas.SetTop(boundingBox, gate.Position.Y - MainWindow.LineRadius);

            Canvas.SetLeft(tagLabel, gate.Position.X);
            Canvas.SetTop(tagLabel, gate.Position.Y);

            Canvas.SetLeft(nameLabel, gate.Position.X);
            Canvas.SetBottom(nameLabel, gate.Position.Y);
            nameLabel.Content = gate.Name;

            if (gate.IsSelected)
            {
                boundingBox.Fill = new SolidColorBrush(Color.FromArgb(30, 16, 92, 255));
                boundingBox.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                boundingBox.Fill = Brushes.Transparent;
                boundingBox.Stroke = Brushes.Black;
            }

            foreach (ConnectionNodeRenderer renderer in connectionNodeRenderers)
            {
                renderer.OnLayoutChanged();
            }
        }

        public void OnConnectionsChanged()
        {
            connectionNodeRenderers.Clear();

            foreach (InputNode inputNode in gate.Input)
                connectionNodeRenderers.Add(inputNode.Renderer);

            foreach (OutputNode outputNode in gate.Output)
                connectionNodeRenderers.Add(outputNode.Renderer);
        }
    }
}
