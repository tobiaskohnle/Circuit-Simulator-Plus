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
    public class CableRenderer
    {
        Cable cable;
        Line line;

        public CableRenderer(Cable cable)
        {
            this.cable = cable;

            line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };

            cable.OnRenderedChanged += OnRenderedChanged;

            if (cable.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (cable.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(line);

                cable.InputNode.OnPositionChanged += OnPositionChanged;
                cable.OutputNode.OnPositionChanged += OnPositionChanged;
                cable.InputNode.OnSelectionChanged += OnSelectionChanged;
                cable.OutputNode.OnSelectionChanged += OnSelectionChanged;
                cable.InputNode.OnEmptyChanged += OnEmptyChanged;
                cable.OutputNode.OnEmptyChanged += OnEmptyChanged;
                cable.OutputNode.OnStateChanged += OnStateChanged;

                OnSelectionChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(line);

                cable.InputNode.OnPositionChanged -= OnPositionChanged;
                cable.OutputNode.OnPositionChanged -= OnPositionChanged;
                cable.InputNode.OnSelectionChanged -= OnSelectionChanged;
                cable.OutputNode.OnSelectionChanged -= OnSelectionChanged;
                cable.InputNode.OnEmptyChanged -= OnEmptyChanged;
                cable.OutputNode.OnEmptyChanged -= OnEmptyChanged;
                cable.OutputNode.OnStateChanged -= OnStateChanged;
            }
        }

        public void OnSelectionChanged()
        {
            if (cable.OutputNode.IsSelected || cable.InputNode.IsSelected)
            {
                line.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                OnStateChanged();
            }
        }

        public void OnEmptyChanged()
        {
            cable.IsRendered = !cable.OutputNode.IsEmpty && !cable.InputNode.IsEmpty;
        }

        public void OnStateChanged()
        {
            line.Stroke = cable.OutputNode.State ? Brushes.Red : Brushes.Black;
        }

        public void OnPositionChanged()
        {
            line.X1 = cable.OutputNode.Position.X;
            line.Y1 = cable.OutputNode.Position.Y;
            line.X2 = cable.InputNode.Position.X;
            line.Y2 = cable.InputNode.Position.Y;
        }
    }
}
