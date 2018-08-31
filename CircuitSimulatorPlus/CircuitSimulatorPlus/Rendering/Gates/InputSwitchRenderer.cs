using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace CircuitSimulatorPlus
{
    public class InputSwitchRenderer
    {
        InputSwitch gate;

        readonly SolidColorBrush activeBrush = new SolidColorBrush(Color.FromArgb(200, 255, 20, 20));
        readonly SolidColorBrush inactiveBrush = new SolidColorBrush(Color.FromArgb(70, 170, 170, 170));

        Rectangle rect = new Rectangle();
        TextBlock indexText = new TextBlock() { FontSize = 1 };

        public InputSwitchRenderer(InputSwitch gate)
        {
            this.gate = gate;

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
                MainWindow.Self.canvas.Children.Add(rect);
                MainWindow.Self.canvas.Children.Add(indexText);
                gate.OnSizeChanged += OnLayoutChanged;
                gate.OnPositionChanged += OnLayoutChanged;
                gate.OnStateChanged += OnStateChanged;
                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(rect);
                MainWindow.Self.canvas.Children.Remove(indexText);
                gate.OnSizeChanged -= OnLayoutChanged;
                gate.OnPositionChanged -= OnLayoutChanged;
                gate.OnStateChanged -= OnStateChanged;
            }
        }

        public void OnStateChanged()
        {
            rect.Fill = gate.State ? activeBrush : inactiveBrush;
        }

        public void OnLayoutChanged()
        {
            rect.Width = gate.Size.Width;
            rect.Height = gate.Size.Height;
            Canvas.SetLeft(rect, gate.Position.X);
            Canvas.SetTop(rect, gate.Position.Y);
            Canvas.SetLeft(indexText, gate.Position.X);
            Canvas.SetTop(indexText, gate.Position.Y);
            indexText.Text = gate.Index.ToString();
        }
    }
}
