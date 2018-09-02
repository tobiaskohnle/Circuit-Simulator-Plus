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
    public class InputSwitchRenderer
    {
        InputSwitch gate;

        readonly SolidColorBrush activeBrush = new SolidColorBrush(Color.FromArgb(200, 255, 20, 20));
        readonly SolidColorBrush inactiveBrush = new SolidColorBrush(Color.FromArgb(70, 170, 170, 170));

        Rectangle rect = new Rectangle();

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
                gate.OnSizeChanged += OnLayoutChanged;
                gate.OnPositionChanged += OnLayoutChanged;
                gate.OnStateChanged += OnStateChanged;
                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(rect);
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
        }
    }
}
