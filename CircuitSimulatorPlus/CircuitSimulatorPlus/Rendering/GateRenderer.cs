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
    /// <summary>
    /// Documentation is elementary.
    /// </summary>
    public class GateRenderer : IRenderer
    {
        Canvas canvas;
        Gate gate;
        Rectangle rectangle;
        Label innerLabel;
        Label outerLabel;
        List<Line> inputLines;
        List<Line> outputLines;
        Ellipse[] inputNegationCircles;
        Ellipse[] outputNegationCircles;
        List<Line>[] connectionLines;
        Dictionary<Gate, Line[]> connectedGateToConnectionLines = new Dictionary<Gate, Line[]>();

        public EventHandler InputClicked;
        public EventHandler OutputClicked;

        public GateRenderer(Canvas canvas, Gate gate)
        {
            this.canvas = canvas;
            this.gate = gate;
        }

        public GateRenderer(Canvas canvas, Gate gate, EventHandler onInputClicked, EventHandler onOutputClicked)
            : this(canvas, gate)
        {
            InputClicked += onInputClicked;
            OutputClicked += onOutputClicked;
        }

        public void Render()
        {
            rectangle = new Rectangle();
            rectangle.Stroke = Brushes.Black;
            rectangle.Margin = new Thickness(MainWindow.LineWidth / -2);
            rectangle.StrokeThickness = MainWindow.LineWidth;
            rectangle.Width = gate.Size.Width + MainWindow.LineWidth;
            rectangle.Height = gate.Size.Height + MainWindow.LineWidth;
            canvas.Children.Add(rectangle);

            outputLines = new List<Line>();
            for (int i = 0; i < gate.Output.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                outputLines.Add(line);
                canvas.Children.Add(line);
            }

            inputLines = new List<Line>();
            for (int i = 0; i < gate.Input.Count; i++)
            {
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                inputLines.Add(line);
                canvas.Children.Add(line);
            }

            if (gate.Name != null)
            {
                outerLabel = new Label();
                outerLabel.Width = gate.Size.Width;
                outerLabel.Height = 1;
                outerLabel.Padding = new Thickness(0);
                outerLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                outerLabel.FontSize = 0.5;
                outerLabel.Content = gate.Name;
                canvas.Children.Add(outerLabel);
            }

            innerLabel = new Label();
            innerLabel.Width = gate.Size.Width;
            innerLabel.Height = gate.Size.Height;
            innerLabel.Padding = new Thickness(0);
            innerLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            innerLabel.FontSize = 1;
            innerLabel.Content = gate.Tag;
            canvas.Children.Add(innerLabel);

            inputNegationCircles = new Ellipse[gate.Input.Count];
            outputNegationCircles = new Ellipse[gate.Output.Count];

            OnConnectionCreated(this, EventArgs.Empty);
            gate.ConnectionCreated += OnConnectionCreated;
            OnInputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnInputChanged;
            OnOutputChanged(this, EventArgs.Empty);
            gate.ConnectionChanged += OnOutputChanged;
            OnPositionChanged(this, EventArgs.Empty);
            gate.PositionChanged += OnPositionChanged;
            OnSelectionChanged(this, EventArgs.Empty);
            gate.SelectionChanged += OnSelectionChanged;
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
            if (outerLabel != null)
                canvas.Children.Remove(outerLabel);
            canvas.Children.Remove(innerLabel);
            foreach (Ellipse inputNegationCircle in inputNegationCircles)
            {
                if (inputNegationCircle != null)
                    canvas.Children.Remove(inputNegationCircle);
            }
            foreach (Ellipse outputNegationCircle in outputNegationCircles)
            {
                if (outputNegationCircle != null)
                    canvas.Children.Remove(outputNegationCircle);
            }
        }

        void OnPositionChanged(object sender, EventArgs e)
        {
            Canvas.SetLeft(rectangle, gate.Position.X);
            Canvas.SetTop(rectangle, gate.Position.Y);

            Canvas.SetLeft(innerLabel, gate.Position.X);
            Canvas.SetTop(innerLabel, gate.Position.Y);

            if (outerLabel != null)
            {
                Canvas.SetLeft(outerLabel, gate.Position.X);
                Canvas.SetBottom(outerLabel, gate.Position.Y);
            }

            for (int i = 0; i < gate.Output.Count; i++)
            {
                OutputNode outputNode = gate.Output[i];
                Line line = outputLines[i];

                if (outputNegationCircles[i] != null)
                {
                    line.X1 = outputNode.Position.X + MainWindow.InversionDotDiameter + MainWindow.LineRadius;
                    Canvas.SetLeft(outputNegationCircles[i], outputNode.Position.X);
                    Canvas.SetTop(outputNegationCircles[i], outputNode.Position.Y - MainWindow.InversionDotRadius - MainWindow.LineRadius);
                }
                else
                {
                    line.X1 = outputNode.Position.X;
                }
                line.X2 = outputNode.Position.X + MainWindow.Unit;

                line.Y1 = outputNode.Position.Y;
                line.Y2 = outputNode.Position.Y;

                for (int j = 0; j < connectionLines[i].Count; j++)
                {
                    connectionLines[i][j].X1 = outputNode.Position.X + MainWindow.Unit;
                    connectionLines[i][j].Y1 = outputNode.Position.Y;
                }
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                InputNode inputNode = gate.Input[i];
                Line line = inputLines[i];

                if (inputNegationCircles[i] != null)
                {
                    line.X1 = inputNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineRadius;
                    Canvas.SetLeft(inputNegationCircles[i], inputNode.Position.X - MainWindow.InversionDotDiameter - MainWindow.LineWidth);
                    Canvas.SetTop(inputNegationCircles[i], inputNode.Position.Y - MainWindow.InversionDotRadius - MainWindow.LineRadius);
                }
                else
                {
                    line.X1 = inputNode.Position.X;
                }
                line.X2 = inputNode.Position.X - MainWindow.Unit;

                line.Y1 = inputNode.Position.Y;
                line.Y2 = inputNode.Position.Y;
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
                    lines[i].X2 = connectedGate.Input[i].Position.X - MainWindow.Unit;
                    lines[i].Y2 = connectedGate.Input[i].Position.Y;
                }
            }
        }

        void OnConnectionCreated(object sender, EventArgs e)
        {
            if (connectionLines != null)
            {
                foreach (List<Line> lines in connectionLines)
                {
                    foreach (Line line in lines)
                    {
                        if (line != null)
                        {
                            canvas.Children.Remove(line);
                        }
                    }
                }
            }
            connectionLines = new List<Line>[gate.Output.Count];
            for (int i = 0; i < gate.Output.Count; i++)
            {
                connectionLines[i] = new List<Line>();
                foreach (InputNode nextNode in gate.Output[i].NextConnectedTo)
                {
                    Line line = new Line();
                    line.Stroke = gate.Output[i].State ? Brushes.Red : Brushes.Black;
                    line.StrokeThickness = MainWindow.LineWidth;
                    line.StrokeStartLineCap = line.StrokeEndLineCap = PenLineCap.Round;
                    line.IsHitTestVisible = false;
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
                    {
                        connectedGateToConnectionLines[nextGate] = new Line[nextGate.Input.Count];
                    }
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
                inputLines[i].Stroke = (gate.Input[i].State != gate.Input[i].IsInverted) ? Brushes.Red : Brushes.Black;
                if (gate.Input[i].IsSelected)
                    inputLines[i].Stroke = SystemColors.MenuHighlightBrush;
                if (gate.Input[i].IsInverted)
                {
                    if (inputNegationCircles[i] == null)
                    {
                        inputNegationCircles[i] = new Ellipse();
                        inputNegationCircles[i].StrokeThickness = MainWindow.LineWidth;
                        inputNegationCircles[i].Width = MainWindow.InversionDotDiameter + MainWindow.LineWidth;
                        inputNegationCircles[i].Height = MainWindow.InversionDotDiameter + MainWindow.LineWidth;
                        canvas.Children.Add(inputNegationCircles[i]);
                    }
                    inputNegationCircles[i].Stroke = gate.Input[i].State ? Brushes.Red : Brushes.Black;
                }
                else if (inputNegationCircles[i] != null)
                {
                    canvas.Children.Remove(inputNegationCircles[i]);
                    inputNegationCircles[i] = null;
                }
            }
            OnPositionChanged(sender, e);
        }

        void OnOutputChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < outputLines.Count; i++)
            {
                outputLines[i].Stroke = gate.Output[i].State ? Brushes.Red : Brushes.Black;
                if (gate.Output[i].IsSelected)
                    outputLines[i].Stroke = SystemColors.MenuHighlightBrush;
                if (gate.Output[i].IsInverted)
                {
                    if (outputNegationCircles[i] == null)
                    {
                        outputNegationCircles[i] = new Ellipse();
                        outputNegationCircles[i].StrokeThickness = MainWindow.LineWidth;
                        outputNegationCircles[i].Width = MainWindow.InversionDotDiameter + MainWindow.LineWidth;
                        outputNegationCircles[i].Height = MainWindow.InversionDotDiameter + MainWindow.LineWidth;
                        canvas.Children.Add(outputNegationCircles[i]);
                    }
                    outputNegationCircles[i].Stroke = !gate.Output[i].State ? Brushes.Red : Brushes.Black;
                }
                else if (outputNegationCircles[i] != null)
                {
                    canvas.Children.Remove(outputNegationCircles[i]);
                    outputNegationCircles[i] = null;
                }
                foreach (Line line in connectionLines[i])
                {
                    line.Stroke = gate.Output[i].State ? Brushes.Red : Brushes.Black;
                }
            }
            OnPositionChanged(sender, e);
        }

        void OnSelectionChanged(object sender, EventArgs e)
        {
            if (gate.IsSelected)
            {
                rectangle.Fill = new SolidColorBrush(Color.FromArgb(30, 16, 92, 255));
                rectangle.Stroke = SystemColors.MenuHighlightBrush;
            }
            else
            {
                rectangle.Fill = Brushes.Transparent;
                rectangle.Stroke = Brushes.Black;
            }
        }
    }
}
