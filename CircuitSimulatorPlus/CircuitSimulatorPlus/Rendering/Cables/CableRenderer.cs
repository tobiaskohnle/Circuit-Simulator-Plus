using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class CableRenderer
    {
        Cable cable;
        List<Line> segments = new List<Line>();

        public CableRenderer(Cable cable)
        {
            this.cable = cable;

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
                UpdateLineSegments();

                cable.OnSelectedChanged += UpdateSegmentColor;
                cable.OnLayoutChanged += OnLayoutChanged;
                cable.OnPointsChanged += UpdateLineSegments;
                MainWindow.Self.OnLastCanvasPosChanged += OnLayoutChanged;
            }
            else
            {
                foreach (Line segment in segments)
                {
                    MainWindow.Self.canvas.Children.Remove(segment);
                }
            }
        }

        public void UpdateSegmentColor()
        {
            SolidColorBrush segmentStroke = cable.State ? Brushes.Red : Brushes.Black;

            foreach (Line segment in segments)
            {
                segment.Stroke = segmentStroke;
            }

            if (cable.IsSelected)
            {
                if (cable.hitbox.SegmentIndex >= 0)
                {
                    segments[cable.hitbox.SegmentIndex].Stroke = SystemColors.MenuHighlightBrush;
                }
            }
        }

        public void UpdateLineSegments()
        {
            int amtSegments = cable.Points.Count + 2;

            while (segments.Count < amtSegments)
            {
                var line = new Line { Stroke = Brushes.Black, StrokeThickness = Constants.LineWidth };
                segments.Add(line);
                MainWindow.Self.canvas.Children.Add(line);
            }
            while (segments.Count > amtSegments)
            {
                segments.RemoveAt(segments.Count - 1);
            }

            OnLayoutChanged();
        }

        public void OnLayoutChanged()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                if ((i & 1) != 0)
                {
                    segments[i].X1 = cable.Points[i + 1];
                    segments[i].Y1 = cable.Points[i];
                    segments[i].X2 = cable.Points[i + 1];
                    segments[i].Y2 = cable.Points[i + 2];
                }
                else
                {
                    segments[i].X1 = cable.Points[i];
                    segments[i].Y1 = cable.Points[i + 1];
                    segments[i].X2 = cable.Points[i + 2];
                    segments[i].Y2 = cable.Points[i + 1];
                }
            }
        }
    }
}
