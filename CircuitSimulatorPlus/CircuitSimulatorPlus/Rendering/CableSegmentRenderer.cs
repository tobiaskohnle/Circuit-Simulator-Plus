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
    public class CableSegmentRenderer
    {
        Canvas canvas;
        CableSegment cableSegment;
        Line line;

        public CableSegmentRenderer(Canvas canvas, CableSegment cableSegment)
        {
            this.canvas = canvas;
            this.cableSegment = cableSegment;

            Render();

            OnSelectionChanged();
            OnPositionChanged();
        }

        public void Render()
        {
            line = new Line
            {
                StrokeThickness = MainWindow.LineWidth
            };

            canvas.Children.Add(line);
        }

        public void Unrender()
        {
            canvas.Children.Remove(line);
        }

        public void OnSelectionChanged()
        {
            if (cableSegment.IsSelected)
            {
                line.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                line.Stroke = cableSegment.State ? Brushes.Red : Brushes.Black;
            }
        }

        public void OnPositionChanged()
        {
            line.X1 = cableSegment.A.Position.X;
            line.Y1 = cableSegment.A.Position.Y;
            line.X2 = cableSegment.B.Position.X;
            line.Y2 = cableSegment.B.Position.Y;
        }
    }
}
