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

        public GateRenderer(Canvas canvas, Gate gate, EventHandler onInputClicked, EventHandler onOutputClicked) : this(canvas, gate)
        {
            InputClicked += onInputClicked;
            OutputClicked += onOutputClicked;
        }

        public void Render()
        {
            rectangle = new Rectangle();
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = MainWindow.LineWidth;
            rectangle.Width = gate.Size.Width;
            rectangle.Height = gate.Size.Height;
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
                outerLabel.Width = 3;
                outerLabel.Height = 1;
                outerLabel.Padding = new Thickness(0);
                outerLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                outerLabel.FontSize = 0.5;
                outerLabel.Content = gate.Name;
                canvas.Children.Add(outerLabel);
            }


            innerLabel = new Label();
            innerLabel.Width = 3;
            innerLabel.Height = 4;
            innerLabel.Padding = new Thickness(0);
            innerLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            innerLabel.FontSize = 1;
            switch (gate.Type)
            {
            case Gate.GateType.And:
                innerLabel.Content = '&';
                break;
            case Gate.GateType.Or:
                innerLabel.Content = "\u22651";
                break;
            case Gate.GateType.Identity:
                innerLabel.Content = '1';
                break;
            }
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

        /*
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
        */

        void OnPositionChanged(object sender, EventArgs e)
        {
            Point pos = gate.Position;
            Canvas.SetLeft(rectangle, pos.X);
            Canvas.SetTop(rectangle, pos.Y);

            Canvas.SetLeft(innerLabel, pos.X);
            Canvas.SetTop(innerLabel, pos.Y);

            if (outerLabel != null)
            {
                Canvas.SetLeft(outerLabel, pos.X);
                Canvas.SetBottom(outerLabel, pos.Y);
            }

            for (int i = 0; i < gate.Output.Count; i++)
            {
                Line line = outputLines[i];
                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.Output.Count);

                if (outputNegationCircles[i] != null)
                {
                    line.X1 = pos.X + 3 + 0.5;
                    Canvas.SetLeft(outputNegationCircles[i], pos.X + 3);
                    Canvas.SetTop(outputNegationCircles[i], y - (0.5 + 0.1) / 2);
                }
                else
                    line.X1 = pos.X + 3;
                line.X2 = pos.X + 3 + 1;

                line.Y1 = y;
                line.Y2 = y;

                for (int j = 0; j < connectionLines[i].Count; j++)
                {
                    connectionLines[i][j].X1 = pos.X + 3 + 1;
                    connectionLines[i][j].Y1 = y;
                }
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                Line line = inputLines[i];
                double y = pos.Y + (double)4 * (1 + 2 * i) / (2 * gate.Input.Count);

                if (inputNegationCircles[i] != null)
                {
                    line.X1 = pos.X - 0.5;
                    Canvas.SetLeft(inputNegationCircles[i], pos.X + 3);
                    Canvas.SetTop(inputNegationCircles[i], y - (0.5 + 0.1) / 2);
                }
                else
                    line.X1 = pos.X;
                line.X2 = pos.X - 1;

                line.Y1 = y;
                line.Y2 = y;
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
                    line.Stroke = outputLines[i].Stroke;
                    line.StrokeThickness = MainWindow.LineWidth / 2;
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
                if (gate.Input[i].IsInverted)
                {
                    if (inputNegationCircles[i] == null)
                    {
                        inputNegationCircles[i] = new Ellipse();
                        inputNegationCircles[i].StrokeThickness = MainWindow.LineWidth;
                        inputNegationCircles[i].Width = 0.5 + 0.1;
                        inputNegationCircles[i].Height = 0.5 + 0.1;
                        canvas.Children.Add(inputNegationCircles[i]);
                    }
                    inputNegationCircles[i].Stroke = !gate.Input[i].State ? Brushes.Red : Brushes.Black;
                }
                else if (inputNegationCircles[i] != null)
                {
                    canvas.Children.Remove(inputNegationCircles[i]);
                    inputNegationCircles[i] = null;
                }
            }
        }

        void OnOutputChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < outputLines.Count; i++)
            {
                outputLines[i].Stroke = gate.Output[i].State ? Brushes.Red : Brushes.Black;
                if (gate.Output[i].IsInverted)
                {
                    if (outputNegationCircles[i] == null)
                    {
                        outputNegationCircles[i] = new Ellipse();
                        outputNegationCircles[i].StrokeThickness = MainWindow.LineWidth;
                        outputNegationCircles[i].Width = 0.5 + 0.1;
                        outputNegationCircles[i].Height = 0.5 + 0.1;
                        canvas.Children.Add(outputNegationCircles[i]);
                    }
                    outputNegationCircles[i].Stroke = outputLines[i].Stroke;
                }
                else if (outputNegationCircles[i] != null)
                {
                    canvas.Children.Remove(outputNegationCircles[i]);
                    outputNegationCircles[i] = null;
                }
                foreach (Line line in connectionLines[i])
                {
                    line.Stroke = outputLines[i].Stroke;
                }
            }
        }

        void OnSelectionChanged(object sender, EventArgs e)
        {
            if (gate.IsSelected)
                rectangle.Stroke = Brushes.Turquoise;
            else
                rectangle.Stroke = Brushes.Black;
        }
    }
}
