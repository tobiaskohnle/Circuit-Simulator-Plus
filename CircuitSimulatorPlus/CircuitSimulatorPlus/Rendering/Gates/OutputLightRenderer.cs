using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus.Rendering
{
    public class OutputLightRenderer
    {
        OutputLight gate;

        Rectangle rect = new Rectangle();

        public OutputLightRenderer(OutputLight gate)
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
                gate.Input[0].OnStateChanged += OnStateChanged;
                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(rect);
                gate.OnSizeChanged -= OnLayoutChanged;
                gate.OnPositionChanged -= OnLayoutChanged;
                gate.Input[0].OnStateChanged -= OnStateChanged;
            }
        }

        public void OnStateChanged()
        {
            rect.Fill = gate.Input[0].State ? MainWindow.Self.Theme.LightHigh : MainWindow.Self.Theme.LightLow;
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
