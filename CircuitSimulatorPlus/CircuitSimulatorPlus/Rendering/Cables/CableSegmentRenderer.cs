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
                MainWindow.Self.OnLastCanvasPosChanged += OnPositionChanged;

                OnSelectedChanged();
                OnPositionChanged();
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
            Point point = cableSegment.Parent.Points[cableSegment.Index];
            Point lastPoint = cableSegment.Parent.Points[Math.Max(0, cableSegment.Index - 1)];
            Point nextPoint = cableSegment.Parent.Points[Math.Min(cableSegment.Parent.Points.Count - 1, cableSegment.Index + 1)];

            bool vert = (cableSegment.Index & 1) != 0;
            line.X1 = vert ? point.X : lastPoint.X;
            line.Y1 = vert ? lastPoint.Y : point.Y;
            line.X2 = vert ? point.X : nextPoint.X;
            line.Y2 = vert ? nextPoint.Y : point.Y;
        }
    }
}
