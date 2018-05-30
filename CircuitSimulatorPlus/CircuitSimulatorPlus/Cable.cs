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
    class Cable
    {
        Canvas canvas;
        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>();
        double positionX1, positionX2, positionY1, positionY2, stateX, stateY;
        public Cable(Canvas canvas, double positionX1, double positionX2, double positionY1, double positionY2)
        {
            this.positionX1 = positionX1;
            this.positionX2 = positionX2;
            this.positionY1 = positionY1;
            this.positionY2 = positionY2;
            this.canvas = canvas;
        }

     //TODO: Cable by drawing fails, if value is bigger as or is negative. 
    public void Draw()
        {
            if (positionX1 < positionX2 && positionX1 > 0)
            {
                stateX = 0;
            }
            else if (positionX1 > positionX2 && positionX2 > 0)
            {
                stateX = 1;
            }
            if (positionX1 < positionX2 && positionX1 < 0)
            {
                stateX = 2;
            }
            else if (positionX1 > positionX2 && positionX2 < 0)
            {
                stateX = 3;
            }
            if (positionY1 < positionY2 && positionY1 > 0)
            {
                stateY = 0;
            }
            else if (positionY1 > positionY2 && positionY2 > 0)
            {
                stateY = 1;
            }
            if (positionY1 < positionY2 && positionY1 < 0)
            {
                stateY = 2;
            }
            else if (positionY1 > positionY2 && positionY2 < 0)
            {
                stateY = 3;
            }
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            switch (stateX)
            {
                case 0:
                    line.X1 = positionX1 + (positionX2 - positionX1) / 2;
                    line.X2 = positionX1 + (positionX2 - positionX1) / 2;
                    break;
                case 1:
                    line.X1 = positionX1 + (positionX1 - positionX2) / 2;
                    line.X2 = positionX1 + (positionX1 - positionX2) / 2;
                    break;
                case 2:
                    line.X1 = positionX1 + (positionX2 + positionX1) / 2;
                    line.X2 = positionX1 + (positionX2 + positionX1) / 2;
                    break;
                case 3:
                    line.X1 = positionX1 + (positionX1 + positionX2) / 2;
                    line.X2 = positionX1 + (positionX1 + positionX2) / 2;
                    break;
            }
            switch (stateY)
            {
                case 0:
                    line.Y1 = positionY1 - MainWindow.LineWidth / 2;
                    line.Y2 = positionY2 + MainWindow.LineWidth / 2;
                    break;
                case 1:
                    line.Y1 = positionY1 + MainWindow.LineWidth / 2;
                    line.Y2 = positionY2 - MainWindow.LineWidth / 2;
                    break;
                case 2:
                    line.Y1 = positionY1 + MainWindow.LineWidth / 2;
                    line.Y2 = positionY2 + MainWindow.LineWidth / 2;
                    break;
                case 3:
                    line.Y1 = positionY1 + MainWindow.LineWidth / 2;
                    line.Y2 = positionY2 + MainWindow.LineWidth / 2;
                    break;
            }

            verticalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            switch (stateX)
            {
                case 0:
                    line.X1 = positionX1 + (positionX2 - positionX1) / 2;
                    line.X2 = positionX1 + (positionX2 - positionX1) / 2;
                    break;
                case 1:
                    line.X1 = positionX1 + (positionX1 - positionX2) / 2;
                    line.X2 = positionX1 + (positionX1 - positionX2) / 2;
                    break;
                case 2:
                    line.X1 = positionX1 + (positionX2 + positionX1) / 2;
                    line.X2 = positionX1 + (positionX2 + positionX1) / 2;
                    break;
                case 3:
                    line.X1 = positionX1 + (positionX1 + positionX2) / 2;
                    line.X2 = positionX1 + (positionX1 + positionX2) / 2;
                    break;
            }
            line.X1 = positionX1;
            line.X2 = positionX1 + 1 / 2 * MainWindow.LineWidth+(positionX2 - positionX1) / 2 ;
            line.Y1 = positionY1;
            line.Y2 = positionY1;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = MainWindow.LineWidth;
            line.X1 = positionX1 - 1 / 2 * MainWindow.LineWidth+(positionX2 - positionX1) / 2;
            line.X2 = positionX2;
            line.Y1 = positionY2;
            line.Y2 = positionY2;
            horizontalLines.Add(line);
            canvas.Children.Add(line);
        }
    }
}
