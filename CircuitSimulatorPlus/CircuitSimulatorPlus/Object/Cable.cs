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
    public class Cable
    {
        public List<Point> Points = new List<Point>();
        public CableRenderer Renderer;

        private OutputNode output;
        private InputNode input;

        public EventHandler StateChanged;

        bool lastSegmentHorizontal = true;

        public OutputNode Output
        {
            get { return output; }

            set
            {
                if (output != null)
                {
                    output.Owner.ConnectionChanged -= OnOutputChanged;
                    output.Owner.PositionChanged -= OnOutputGateMoved;
                }
                output = value;
                output.Owner.ConnectionChanged += OnOutputChanged;
                output.Owner.PositionChanged += OnOutputGateMoved;
            }
        }

        public InputNode Input
        {
            get { return input; }

            set
            {
                if (input != null)
                    input.Owner.PositionChanged -= OnInputGateMoved;
                input = value;
                input.Owner.PositionChanged += OnInputGateMoved;
            }
        }

        public void AddPoint(Point point, bool toConnection = false)
        {
            if (Points.Count > 0)
            {
                Point lastPoint = Points.Last();
                if (point.X != lastPoint.X && point.Y != lastPoint.Y)
                {
                    bool horizontalWouldOverlap = false, verticalWouldOverlap = false;
                    if (Points.Count > 1)
                    {
                        Point lastLastPoint = Points[Points.Count - 2];
                        if (lastSegmentHorizontal)
                        {
                            if (lastLastPoint.X < lastPoint.X)
                                horizontalWouldOverlap = point.X < lastPoint.X;
                            else
                                horizontalWouldOverlap = point.X > lastPoint.X;
                        }
                        else
                        {
                            if (lastLastPoint.Y < lastPoint.Y)
                                verticalWouldOverlap = point.Y < lastPoint.Y;
                            else
                                verticalWouldOverlap = point.Y > lastPoint.Y;
                        }
                    }

                    if (toConnection && lastSegmentHorizontal && !horizontalWouldOverlap)
                    {
                        double midX = (lastPoint.X + point.X) / 2;
                        Points.Add(new Point(midX, lastPoint.Y));
                        Points.Add(new Point(midX, point.Y));
                    }
                    else
                    {
                        Point cornerPoint;
                        if (verticalWouldOverlap || (lastSegmentHorizontal && !horizontalWouldOverlap))
                            cornerPoint = new Point(point.X, lastPoint.Y);
                        else
                            cornerPoint = new Point(lastPoint.X, point.Y);
                        Points.Add(cornerPoint); 
                    }
                }
                lastSegmentHorizontal = point.Y == Points.Last().Y;
            }
            Points.Add(point);
            if (Renderer != null)
                Renderer.Update();
        }

        void OnOutputChanged(object sender, EventArgs e)
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        void OnOutputGateMoved(object sender, EventArgs e)
        {
            Gate outputGate = Output.Owner;
            int outputIndex = 0;
            for (int i = 0; i < outputGate.Output.Count; i++)
            {
                if (outputGate.Output[i] == Output)
                {
                    outputIndex = i;
                    break;
                }
            }
            Point inputPos = Points.Last();
            var outputPos = new Point(outputGate.Position.X + 3 + 1, outputGate.Position.Y + (double)4 * (1 + 2 * outputIndex) / (2 * outputGate.Output.Count));
            Points.Clear();
            lastSegmentHorizontal = true;
            AddPoint(outputPos, true);
            AddPoint(inputPos, true);
            Renderer.Update();
        }

        void OnInputGateMoved(object sender, EventArgs e)
        {
            Gate inputGate = Input.Owner;
            int inputIndex = 0;
            for (int i = 0; i < inputGate.Input.Count; i++)
            {
                if (inputGate.Input[i] == Input)
                {
                    inputIndex = i;
                    break;
                }
            }
            Point outputPos = Points.First();
            Point inputPos = new Point(inputGate.Position.X - 1, inputGate.Position.Y + (double)4 * (1 + 2 * inputIndex) / (2 * inputGate.Input.Count));
            Points.Clear();
            lastSegmentHorizontal = true;
            AddPoint(outputPos, true);
            AddPoint(inputPos, true);
            Renderer.Update();
        }
    }
}
