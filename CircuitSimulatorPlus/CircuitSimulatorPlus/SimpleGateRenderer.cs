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
    public class SimpleGateRenderer : IRenderer
    {
        Canvas canvas;
        Gate gate;
        Rectangle rectangle;
        List<Line> inputLines = new List<Line>();
        List<Line> outputLines = new List<Line>();
        List<Line>[] connectionLines;
        Dictionary<Gate, Line[]> connectedGateToConnectionLines = new Dictionary<Gate, Line[]>();

        List<Rectangle> outputHitboxes = new List<Rectangle>();
        List<Rectangle> inputHitboxes = new List<Rectangle>();

        public EventHandler InputClicked;
        public EventHandler OutputClicked;

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
                Rectangle hitbox = new Rectangle();
                hitbox.Fill = new SolidColorBrush(Color.FromArgb(63, 255, 0, 0));
                hitbox.Width = 1;
                hitbox.Height = 1;
                hitbox.MouseDown += OnOutputClicked;
                outputHitboxes.Add(hitbox);
                canvas.Children.Add(hitbox);
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                inputLines.Add(line);
                canvas.Children.Add(line);
                Rectangle hitbox = new Rectangle();
                hitbox.Fill = new SolidColorBrush(Color.FromArgb(63, 255, 0, 0));
                hitbox.Width = 1;
                hitbox.Height = 1;
                hitbox.MouseDown += OnInputClicked;
                inputHitboxes.Add(hitbox);
                canvas.Children.Add(hitbox);
            }

            OnConnectionCreated(this, EventArgs.Empty);
            gate.ConnectionCreated += OnConnectionCreated;
            OnInputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnInputChanged;
            OnOutputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnOutputChanged;
            OnPositionChanged(this, EventArgs.Empty);
            gate.PositionChanged += OnPositionChanged;
        }

        public void Unrender()
        {
            canvas.Children.Remove(rectangle);
            foreach (Line line in outputLines)
                canvas.Children.Remove(line);
            foreach (Line line in inputLines)
                canvas.Children.Remove(line);
            foreach (List<Line> lines in connectionLines)
            {
                foreach (Line line in lines)
                {
                    if (line != null)
                        canvas.Children.Remove(line);
                }
            }
        }

        void OnInputClicked(object sender, EventArgs e)
        {
            int index = 0;
            for (int i = 0; i < inputHitboxes.Count; i++)
            {
                if (sender == inputHitboxes[i])
                {
                    index = i;
                    break;
                }
            }
            IndexEventArgs args = new IndexEventArgs(index);
            InputClicked?.Invoke(gate, args);
        }

        void OnOutputClicked(object sender, EventArgs e)
        {
            int index = 0;
            for (int i = 0; i < outputHitboxes.Count; i++)
            {
                if (sender == outputHitboxes[i])
                {
                    index = i;
                    break;
                }
            }
            IndexEventArgs args = new IndexEventArgs(index);
            OutputClicked?.Invoke(gate, args);
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

                for (int j = 0; j < connectionLines[i].Count; j++)
                {
                    connectionLines[i][j].X1 = pos.X + 3 + 1;
                    connectionLines[i][j].Y1 = y;
                }

                Canvas.SetLeft(outputHitboxes[i], pos.X + 3);
                Canvas.SetTop(outputHitboxes[i], y - 0.5);
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                Line line = inputLines[i];
                line.X1 = pos.X;
                line.X2 = pos.X - 1;

                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.Input.Count);
                line.Y1 = y;
                line.Y2 = y;

                Canvas.SetLeft(inputHitboxes[i], pos.X - 1);
                Canvas.SetTop(inputHitboxes[i], y - 0.5);
            }
        }

        void OnConnectedGatePositionChanged(object sender, EventArgs e)
        {
            Gate connectedGate = (Gate)sender;
            Line[] lines = connectedGateToConnectionLines[connectedGate];
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null)
                {
                    lines[i].X2 = connectedGate.Position.X - 1;
                    lines[i].Y2 = connectedGate.Position.Y + (double)4 * (1 + 2 * i) / (2 * connectedGate.Input.Count);
                }
            }
        }

        void OnConnectionCreated(object sender, EventArgs e)
        {
            connectionLines = new List<Line>[gate.Output.Count];
            for (int i = 0; i < gate.Output.Count; i++)
            {
                if (connectionLines[i] != null)
                {
                    foreach (Line line in connectionLines[i])
                    {
                        canvas.Children.Remove(line);
                    }
                }
                connectionLines[i] = new List<Line>();
                foreach (InputNode nextNode in gate.Output[i].NextConnectedTo)
                {
                    Line line = new Line();
                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = MainWindow.LineWidth / 2;
                    connectionLines[i].Add(line);
                    canvas.Children.Add(line);

                    Gate nextGate = nextNode.Owner;
                    int nextNodeIndex = 0;
                    for (int j = 0; j < nextGate.Input.Count; j++)
                    {
                        if (nextGate.Input[j] == nextNode)
                        {
                            nextNodeIndex = j;
                            break;
                        }
                    }
                    if (!connectedGateToConnectionLines.ContainsKey(nextGate))
                        connectedGateToConnectionLines[nextGate] = new Line[nextGate.Input.Count];
                    connectedGateToConnectionLines[nextGate][nextNodeIndex] = line;

                    OnPositionChanged(this, EventArgs.Empty);
                    OnConnectedGatePositionChanged(nextGate, EventArgs.Empty);
                    nextGate.PositionChanged += OnConnectedGatePositionChanged;
                }
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
                foreach (Line line in connectionLines[i])
                {
                    line.Stroke = outputLines[i].Stroke;
                }
            }
        }
    }
}
