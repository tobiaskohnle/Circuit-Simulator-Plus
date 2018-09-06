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
    public class GateRenderer
    {
        Gate gate;

        Rectangle boundingBox;
        Label nameLabel;
        Label tagLabel;

        public GateRenderer(Gate gate)
        {
            this.gate = gate;

            boundingBox = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };
            Panel.SetZIndex(boundingBox, 2);

            nameLabel = new Label
            {
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = 0.5
            };
            Panel.SetZIndex(nameLabel, 5);

            tagLabel = new Label
            {
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = 1
            };
            Panel.SetZIndex(tagLabel, 6);

            gate.OnRenderedChanged += OnRenderedChanged;

            if (gate.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (gate.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(nameLabel);
                MainWindow.Self.canvas.Children.Add(tagLabel);
                MainWindow.Self.canvas.Children.Add(boundingBox);

                gate.OnSelectionChanged += OnSelectionChanged;
                gate.OnNameChanged += OnNameChanged;
                gate.OnTagChanged += OnTagChanged;
                gate.OnSizeChanged += OnSizeChanged;
                gate.OnPositionChanged += OnPositionChanged;

                OnSelectionChanged();
                OnNameChanged();
                OnTagChanged();
                OnSizeChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(tagLabel);
                MainWindow.Self.canvas.Children.Remove(nameLabel);
                MainWindow.Self.canvas.Children.Remove(boundingBox);

                gate.OnSelectionChanged -= OnSelectionChanged;
                gate.OnNameChanged -= OnNameChanged;
                gate.OnTagChanged -= OnTagChanged;
                gate.OnSizeChanged -= OnSizeChanged;
                gate.OnPositionChanged -= OnPositionChanged;
                gate.OnRenderedChanged -= OnRenderedChanged;
            }
        }

        public void OnSelectionChanged()
        {
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
        }

        public void OnNameChanged()
        {
            nameLabel.Content = gate.Name;
        }

        public void OnTagChanged()
        {
            tagLabel.Content = gate.Tag;
        }

        public void OnSizeChanged()
        {
            boundingBox.Width = gate.Size.Width + Constants.LineWidth;
            boundingBox.Height = gate.Size.Height + Constants.LineWidth;
            nameLabel.Width = gate.Size.Width;
            nameLabel.Height = 1;
            tagLabel.Width = gate.Size.Width;
            tagLabel.Height = gate.Size.Height;
        }

        public void OnPositionChanged()
        {
            Canvas.SetLeft(boundingBox, gate.Position.X - Constants.LineRadius);
            Canvas.SetTop(boundingBox, gate.Position.Y - Constants.LineRadius);

            Canvas.SetLeft(tagLabel, gate.Position.X);
            Canvas.SetTop(tagLabel, gate.Position.Y);

            Canvas.SetLeft(nameLabel, gate.Position.X);
            Canvas.SetTop(nameLabel, gate.Position.Y - nameLabel.Height);
        }
    }
}
