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
        Canvas canvas;
        Gate gate;

        Rectangle boundingBox;
        Label tagLabel;
        Label nameLabel;

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
                StrokeThickness = MainWindow.LineWidth
            };

            nameLabel = new Label
            {
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = MainWindow.Unit / 2
            };

            tagLabel = new Label
            {
                Padding = new Thickness(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = MainWindow.Unit
            };

            canvas.Children.Add(boundingBox);
            canvas.Children.Add(nameLabel);
            canvas.Children.Add(tagLabel);

            OnSelectionChanged();
            OnNameChanged();
            OnTagChanged();
            OnSizeChanged();
            OnPositionChanged();
        }

        public void Unrender()
        {
            canvas.Children.Remove(boundingBox);
            canvas.Children.Remove(nameLabel);
            canvas.Children.Remove(tagLabel);
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
            boundingBox.Width = gate.Size.Width + MainWindow.LineWidth;
            boundingBox.Height = gate.Size.Height + MainWindow.LineWidth;
            nameLabel.Width = gate.Size.Width;
            nameLabel.Height = MainWindow.Unit;
            tagLabel.Width = gate.Size.Width;
            tagLabel.Height = gate.Size.Height;
        }

        public void OnPositionChanged()
        {
            Canvas.SetLeft(boundingBox, gate.Position.X - MainWindow.LineRadius);
            Canvas.SetTop(boundingBox, gate.Position.Y - MainWindow.LineRadius);

            Canvas.SetLeft(tagLabel, gate.Position.X);
            Canvas.SetTop(tagLabel, gate.Position.Y);

            Canvas.SetLeft(nameLabel, gate.Position.X);
            Canvas.SetBottom(nameLabel, gate.Position.Y);
        }
    }
}
