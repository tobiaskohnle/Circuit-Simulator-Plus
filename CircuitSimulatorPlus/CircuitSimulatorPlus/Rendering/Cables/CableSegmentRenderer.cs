﻿using System.Windows;
using System.Windows.Controls;
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
                Stroke = MainWindow.Self.Theme.MainColor,
                StrokeThickness = Constants.LineWidth,
                StrokeStartLineCap = PenLineCap.Triangle
            };
            Panel.SetZIndex(line, 0);

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

                cableSegment.OnSelectedChanged += OnStateChanged;
                cableSegment.Parent.OnStateChanged += OnStateChanged;
                cableSegment.Parent.OnPointsChanged += OnPositionChanged;
                MainWindow.Self.OnLastCanvasPosChanged += OnPositionChanged;

                OnStateChanged();
                OnPositionChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(line);

                cableSegment.OnSelectedChanged -= OnStateChanged;
                cableSegment.Parent.OnStateChanged -= OnStateChanged;
                cableSegment.Parent.OnPointsChanged -= OnPositionChanged;
                MainWindow.Self.OnLastCanvasPosChanged -= OnPositionChanged;
            }
        }

        public void OnStateChanged()
        {
            if (cableSegment.IsSelected)
            {
                line.Stroke = MainWindow.Self.Theme.SelectedHighlight;
            }
            else
            {
                line.Stroke = cableSegment.Parent.State ? MainWindow.Self.Theme.High : MainWindow.Self.Theme.Low;
            }
        }

        public void OnPositionChanged()
        {
            if (cableSegment.Parent.IsCompleted)
            {
                MainWindow.Self.OnLastCanvasPosChanged -= OnPositionChanged;
            }

            Point point = cableSegment.Parent.GetPoint(cableSegment.Index);
            Point lastPoint = cableSegment.Parent.GetPoint(cableSegment.Index - 1);
            Point nextPoint = cableSegment.Parent.GetPoint(cableSegment.Index + 1);

            bool vert = (cableSegment.Index & 1) != 0;
            line.X1 = vert ? point.X : lastPoint.X;
            line.Y1 = vert ? lastPoint.Y : point.Y;
            line.X2 = vert ? point.X : nextPoint.X;
            line.Y2 = vert ? nextPoint.Y : point.Y;
        }
    }
}
