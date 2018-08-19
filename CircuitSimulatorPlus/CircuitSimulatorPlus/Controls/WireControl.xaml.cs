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
    /// <summary>
    /// Interaktionslogik für WireControl.xaml
    /// </summary>
    public partial class WireControl : UserControl
    {
        public event EventHandler SelectionChanged;

        private bool selected = true;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    Visibility ellipseVisibility = selected ? Visibility.Visible : Visibility.Collapsed;
                    startEllipse.Visibility = ellipseVisibility;
                    endEllipse.Visibility = ellipseVisibility;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Point StartPoint
        {
            get
            {
                return new Point(Canvas.GetLeft(startEllipse), Canvas.GetTop(startEllipse));
            }

            set
            {
                Canvas.SetLeft(startEllipse, value.X - startEllipse.Width / 2);
                Canvas.SetTop(startEllipse, value.Y - startEllipse.Height / 2);
                line.X1 = value.X + startEllipse.Width / 2;
                line.Y1 = value.Y + startEllipse.Height / 2;
            }
        }

        public Point EndPoint
        {
            get
            {
                return new Point(Canvas.GetLeft(endEllipse), Canvas.GetTop(endEllipse));
            }

            set
            {
                Canvas.SetLeft(endEllipse, value.X - endEllipse.Width / 2);
                Canvas.SetTop(endEllipse, value.Y - endEllipse.Height / 2);
                line.X2 = value.X + endEllipse.Width / 2;
                line.Y2 = value.Y + endEllipse.Height / 2;
            }
        }

        public WireControl()
        {
            InitializeComponent();
        }


        private Ellipse dragEllipse;
        private Window parentWindow;

        private void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            dragEllipse = ellipse;
            CaptureMouse();
            e.Handled = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            dragEllipse = null;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragEllipse != null)
            {
                Point pos = e.GetPosition(this);
                Canvas.SetLeft(dragEllipse, pos.X - dragEllipse.Width / 2);
                Canvas.SetTop(dragEllipse, pos.Y - dragEllipse.Height / 2);
                line.X1 = Canvas.GetLeft(startEllipse) + startEllipse.Width / 2;
                line.Y1 = Canvas.GetTop(startEllipse) + startEllipse.Height / 2;
                line.X2 = Canvas.GetLeft(endEllipse) + endEllipse.Width / 2;
                line.Y2 = Canvas.GetTop(endEllipse) + endEllipse.Height / 2;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            parentWindow = ((this.Parent as Canvas)?.Parent as Grid)?.Parent as Window;
        }

        private void line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected = true;
        }

        private void ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            ellipse.RenderTransform = new ScaleTransform(1.1, 1.1, 0.5, 0.5);
        }

        private void ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            ellipse.RenderTransform = Transform.Identity;
        }
    }
}
