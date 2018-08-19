using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus.Controls
{
    public partial class GateControl : UserControl
    {
        public GateControl(Gate gate)
        {
            InitializeComponent();

            this.gate = gate;

            name = gate.Name;
            tag = gate.Tag;
            isSelected = gate.IsSelected;
            size = gate.Size;
        }

        Gate gate;

        public new void MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                nameLabel.Content = value;
            }
        }

        string tag;
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
                tagLabel.Content = value;
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if (value)
                {
                    boundingBox.Fill = new SolidColorBrush(Color.FromArgb(30, 16, 92, 255));
                    boundingBox.Stroke = SystemColors.MenuHighlightBrush;
                }
                else
                {
                    boundingBox.Fill = Brushes.Transparent;
                    boundingBox.Stroke = Brushes.Black;
                }
            }
        }

        Size size;
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                boundingBox.Width = value.Width + MainWindow.LineWidth;
                boundingBox.Height = value.Height + MainWindow.LineWidth;
                nameLabel.Width = value.Width;
                nameLabel.Height = 1;
                tagLabel.Width = value.Width;
                tagLabel.Height = value.Height;
            }
        }

        Point position;
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Canvas.SetLeft(boundingBox, value.X - MainWindow.LineRadius);
                Canvas.SetTop(boundingBox, value.Y - MainWindow.LineRadius);

                Canvas.SetLeft(tagLabel, value.X);
                Canvas.SetTop(tagLabel, value.Y);

                Canvas.SetLeft(nameLabel, value.X);
                Canvas.SetTop(nameLabel, value.Y - nameLabel.Height);
            }
        }
    }
}
