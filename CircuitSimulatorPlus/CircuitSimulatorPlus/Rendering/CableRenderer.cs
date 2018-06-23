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
    public class CableRenderer
    {
        Canvas canvas;
        Cable cable;
        List<Line> lines = new List<Line>();

        InputNode inputNode;
        OutputNode outputNode;

        public CableRenderer(Canvas canvas, Cable cable, InputNode inputNode, OutputNode outputNode)
        {
            this.canvas = canvas;
            this.cable = cable;

            Render();
        }

        public void Render()
        {
            for (int i = 1; i < cable.Points.Count; i++)
            {
                Point pos1 = cable.Points[i - 1];
                Point pos2 = cable.Points[i];
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = MainWindow.LineWidth;
                line.X1 = pos1.X;
                line.Y1 = pos1.Y;
                line.X2 = pos2.X;
                line.Y2 = pos2.Y;
                canvas.Children.Add(line);
                lines.Add(line);
            }
        }

        public void Unrender()
        {
            foreach (Line line in lines)
                canvas.Children.Remove(line);
        }
/*
        public void OnLayoutChanged()
        {
            Brush brush = cable.Output.State ? Brushes.Red : Brushes.Black;
            foreach (Line line in lines)
            {
                line.Stroke = brush;
            }

            Gate outputGate = outputNode.Owner;
            int outputIndex = 0;
            for (int i = 0; i < outputGate.Output.Count; i++)
            {
                if (outputGate.Output[i] == outputNode)
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


            Gate inputGate = inputNode.Owner;
            int inputIndex = 0;
            for (int i = 0; i < inputGate.Input.Count; i++)
            {
                if (inputGate.Input[i] == inputNode)
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
        }
*/
    }
}
