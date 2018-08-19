using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace CircuitSimulatorPlus
{
    class OutputLightRenderer
    {
        InputSwitch gate;


        readonly SolidColorBrush activeBrush = new SolidColorBrush(Color.FromArgb(200, 255, 20, 20));
        readonly SolidColorBrush inactiveBrush = new SolidColorBrush(Color.FromArgb(70, 170, 170, 170));

        Rectangle Rect = new Rectangle();

        public OutputLightRenderer(InputSwitch gate)
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
                MainWindow.Self.canvas.Children.Add(Rect);

                gate.OnSizeChanged += OnLayoutChanged;
                gate.OnPositionChanged += OnLayoutChanged;

                gate.Input[0].OnStateChanged += OnStateChanged;
                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(Rect);

                gate.OnSizeChanged -= OnLayoutChanged;
                gate.OnPositionChanged -= OnLayoutChanged;

                gate.Input[0].OnStateChanged -= OnStateChanged;
            }
        }

        public void OnStateChanged()
        {
            Rect.Fill = gate.Input[0].State ? activeBrush : inactiveBrush;
        }

        public void OnLayoutChanged()
        {
            Rect.Width = gate.Size.Width;
            Rect.Height = gate.Size.Height;

            
        }
    }
}
