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
        List<Line> connectionLines = new List<Line>();

        public SimpleGateRenderer(Canvas canvas, Gate gate)
        {
            this.canvas = canvas;
            this.gate = gate;
        }

        public void Render()
        {
            rectangle = new Rectangle();
            rectangle.Stroke = Brushes.Black;
            // TODO: rectangle border is inset -> change to centered
            rectangle.StrokeThickness = MainWindow.LineWidth;
            rectangle.Width = 3;
            rectangle.Height = 4;
            canvas.Children.Add(rectangle);

            for (int i = 0; i < gate.Output.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                outputLines.Add(line);
                canvas.Children.Add(line);
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                inputLines.Add(line);
                canvas.Children.Add(line);
            }

            OnPositionChanged(this, EventArgs.Empty);
            gate.PositionChanged += OnPositionChanged;
            OnInputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnInputChanged;
            OnOutputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnOutputChanged;
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

            for (int i = 0; i < gate.Output.Count; i++)
            {
                Line line = outputLines[i];
                line.X1 = pos.X + 3;
                line.X2 = pos.X + 3 + 1;

                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.Output.Count);
                line.Y1 = y;
                line.Y2 = y;
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                Line line = inputLines[i];
                line.X1 = pos.X;
                line.X2 = pos.X - 1;

                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.Input.Count);
                line.Y1 = y;
                line.Y2 = y;
            }
        }

        void OnInputChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < inputLines.Count; i++)
            {
                inputLines[i].Stroke = gate.Input[i].State ? Brushes.Red : Brushes.Black;
            }
        }

        void OnOutputChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < outputLines.Count; i++)
            {
                outputLines[i].Stroke = gate.Output[i].State ? Brushes.Red : Brushes.Black;
                if (!gate.Output[i].IsEmpty)
                {
                    connectionLines[i].Stroke = outputLines[i].Stroke;
                }
            }
        }
    }
}
