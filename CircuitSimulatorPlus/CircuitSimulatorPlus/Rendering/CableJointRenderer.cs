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
    public class CableJointRenderer
    {
        Canvas canvas;
        CableJoint cableJoint;
        Ellipse ellipse;

        public CableJointRenderer(Canvas canvas, CableJoint cableJoint)
        {
            this.canvas = canvas;
            this.cableJoint = cableJoint;

            Render();

            OnSelectionChanged();
            OnPositionChanged();
        }

        public void Render()
        {
            ellipse = new Ellipse
            {
                Height = MainWindow.CableJointSize,
                Width = MainWindow.CableJointSize
            };

            canvas.Children.Remove(ellipse);
        }

        public void Unrender()
        {
            canvas.Children.Remove(ellipse);
        }

        public void OnSelectionChanged()
        {
            if (cableJoint.IsSelected)
            {
                ellipse.Fill = SystemColors.MenuHighlightBrush;
            }
            else
            {
                ellipse.Fill = Brushes.Black;
            }
        }

        public void OnPositionChanged()
        {
            Canvas.SetTop(ellipse, cableJoint.Position.X - MainWindow.CableJointSize / 2);
            Canvas.SetLeft(ellipse, cableJoint.Position.Y - MainWindow.CableJointSize / 2);
        }
    }
}
