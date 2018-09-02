using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class CableSegmentRenderer
    {
        CableSegment cableSegment;

        Line line;

        public CableSegmentRenderer(CableSegment cableSegment)
        {
            this.cableSegment = cableSegment;

            line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = Constants.LineWidth
            };

            cableSegment.OnRenderedChanged += OnRenderedChanged;

            if (cableSegment.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (cableSegment.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(line);

                cableSegment.OnSelectedChanged += OnSelectedChanged;

                OnSelectedChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(line);

                cableSegment.OnSelectedChanged -= OnSelectedChanged;
            }
        }

        public void OnSelectedChanged()
        {
            if (cableSegment.IsSelected)
            {
                line.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                line.Stroke = Brushes.Black;
            }
        }

        public void OnPositionChanged()
        {
            if ((cableSegment.Index & 1) != 0)
            {
                line.X1 = cableSegment.Points[cableSegment.Index].X;
                line.Y1 = cableSegment.Points[cableSegment.Index - 1].Y;
                line.X2 = cableSegment.Points[cableSegment.Index].X;
                line.Y2 = cableSegment.Points[cableSegment.Index + 1].Y;
            }
            else
            {
                line.X1 = cableSegment.Points[cableSegment.Index - 1].X;
                line.Y1 = cableSegment.Points[cableSegment.Index].Y;
                line.X2 = cableSegment.Points[cableSegment.Index + 1].X;
                line.Y2 = cableSegment.Points[cableSegment.Index].Y;
            }
        }
    }
}
