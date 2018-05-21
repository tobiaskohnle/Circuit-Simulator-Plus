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
    public class SimpleGateRenderer : IRenderer
    {
        Canvas canvas;
        Gate gate;
        Rectangle rectangle;
        List<Line> inputLines = new List<Line>();
        List<Line> outputLines = new List<Line>();

        public SimpleGateRenderer(Canvas canvas, Gate gate)
        {
            this.canvas = canvas;
            this.gate = gate;
        }

        public void Render()
        {
            rectangle = new Rectangle();
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = MainWindow.LineWidth;
            rectangle.Width = 3;
            rectangle.Height = 4;
            canvas.Children.Add(rectangle);

            for (int i = 0; i < gate.output.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                outputLines.Add(line);
                canvas.Children.Add(line);
            }

            for (int i = 0; i < gate.input.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                inputLines.Add(line);
                canvas.Children.Add(line);
            }

            OnPositionChanged(this, EventArgs.Empty);
            gate.PositionChanged += OnPositionChanged;
        }

        public void Unrender()
        {
            canvas.Children.Remove(rectangle);
            foreach (Line line in outputLines)
                canvas.Children.Remove(line);
        }

        void OnPositionChanged(object sender, EventArgs e)
        {
            Point pos = gate.Position;
            Canvas.SetLeft(rectangle, pos.X);
            Canvas.SetTop(rectangle, pos.Y);

            for (int i = 0; i < gate.output.Count; i++)
            {
                Line line = outputLines[i];
                line.X1 = pos.X + 3;
                line.X2 = pos.X + 3 + 1;

                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.output.Count);
                line.Y1 = y;
                line.Y2 = y;
            }

            for (int i = 0; i < gate.input.Count; i++)
            {
                Line line = inputLines[i];
                line.X1 = pos.X;
                line.X2 = pos.X - 1;

                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.input.Count);
                line.Y1 = y;
                line.Y2 = y;
            }
        }
    }
}
