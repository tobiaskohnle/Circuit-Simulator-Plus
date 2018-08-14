using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class SegmentDisplayRenderer
    {
        SegmentDisplay gate;

        const double scale = 0.45;
        const double skewX = 1;
        const double width = 2;
        const double totalWidth = 8;
        const double x = 1.55;
        const double segmentScale = 0.97;

        readonly SolidColorBrush activeBrush = new SolidColorBrush(Color.FromArgb(200, 255, 20, 20));
        readonly SolidColorBrush inactiveBrush = new SolidColorBrush(Color.FromArgb(100, 170, 170, 170));

        Polygon top = new Polygon();
        Polygon topLeft = new Polygon();
        Polygon topRight = new Polygon();
        Polygon center = new Polygon();
        Polygon botLeft = new Polygon();
        Polygon botRight = new Polygon();
        Polygon bot = new Polygon();

        public SegmentDisplayRenderer(SegmentDisplay gate)
        {
            this.gate = gate;

            gate.OnRenderedChanged += OnRenderedChanged;
            gate.OnSizeChanged += OnLayoutChanged;
            gate.OnPositionChanged += OnLayoutChanged;

            gate.Input[0].OnStateChanged += OnStateChanged;
            gate.Input[1].OnStateChanged += OnStateChanged;
            gate.Input[2].OnStateChanged += OnStateChanged;
            gate.Input[3].OnStateChanged += OnStateChanged;
            gate.Input[4].OnStateChanged += OnStateChanged;
            gate.Input[5].OnStateChanged += OnStateChanged;
            gate.Input[6].OnStateChanged += OnStateChanged;

            if (gate.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (gate.IsRendered)
            {
                MainWindow.Canvas.Children.Add(top);
                MainWindow.Canvas.Children.Add(topLeft);
                MainWindow.Canvas.Children.Add(topRight);
                MainWindow.Canvas.Children.Add(center);
                MainWindow.Canvas.Children.Add(botLeft);
                MainWindow.Canvas.Children.Add(botRight);
                MainWindow.Canvas.Children.Add(bot);

                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Canvas.Children.Remove(top);
                MainWindow.Canvas.Children.Remove(topLeft);
                MainWindow.Canvas.Children.Remove(topRight);
                MainWindow.Canvas.Children.Remove(center);
                MainWindow.Canvas.Children.Remove(botLeft);
                MainWindow.Canvas.Children.Remove(botRight);
                MainWindow.Canvas.Children.Remove(bot);
            }
        }

        Point TransformPoint(double x, double y, double centerX, double centerY)
        {
            x = centerX + (x - centerX) * segmentScale;
            y = centerY + (y - centerY) * segmentScale;

            x *= scale;
            y *= scale;

            double offX = gate.Position.X + gate.Size.Width / 2;
            double offY = gate.Position.Y + gate.Size.Height / 2;

            x += offX;
            y += offY;

            x += -(y - offY) * skewX / (totalWidth - width / 2);

            return new Point(x, y);
        }

        public void OnStateChanged()
        {
            top.Fill = gate.Input[0].State ? activeBrush : inactiveBrush;
            topRight.Fill = gate.Input[1].State ? activeBrush : inactiveBrush;
            botRight.Fill = gate.Input[2].State ? activeBrush : inactiveBrush;
            bot.Fill = gate.Input[3].State ? activeBrush : inactiveBrush;
            botLeft.Fill = gate.Input[4].State ? activeBrush : inactiveBrush;
            topLeft.Fill = gate.Input[5].State ? activeBrush : inactiveBrush;
            center.Fill = gate.Input[6].State ? activeBrush : inactiveBrush;
        }

        public void OnLayoutChanged()
        {
            double hw = width / 2;

            double d = width - x;

            double h = d * hw / x;

            double iw = totalWidth - 2 * width;
            double ir = iw / 2;

            double cw = ir + hw;
            double ch = hw + hw + iw;

            center.Points = new PointCollection {
                TransformPoint(ir, hw, 0, 0),
                TransformPoint(ir + x, 0, 0, 0),
                TransformPoint(ir, -hw, 0, 0),
                TransformPoint(-ir, -hw, 0, 0),
                TransformPoint(-ir - x, 0, 0, 0),
                TransformPoint(-ir, hw, 0, 0)
            };

            botRight.Points = new PointCollection {
                TransformPoint(ir, hw, cw, cw),
                TransformPoint(ir + x, 0, cw, cw),
                TransformPoint(ir + width, h, cw, cw),
                TransformPoint(ir + width, hw + iw + x - d, cw, cw),
                TransformPoint(ir + x, hw + iw + x, cw, cw),
                TransformPoint(ir, hw + iw, cw, cw)
            };
            botLeft.Points = new PointCollection {
                TransformPoint(-ir, hw, -cw, cw),
                TransformPoint(-ir - x, 0, -cw, cw),
                TransformPoint(-ir - width, h, -cw, cw),
                TransformPoint(-ir - width, hw + iw + x - d, -cw, cw),
                TransformPoint(-ir - x, hw + iw + x, -cw, cw),
                TransformPoint(-ir, hw + iw, -cw, cw)
            };
            topRight.Points = new PointCollection {
                TransformPoint(ir, -hw, cw, -cw),
                TransformPoint(ir + x, 0, cw, -cw),
                TransformPoint(ir + width, -h, cw, -cw),
                TransformPoint(ir + width, -hw - iw - x + d, cw, -cw),
                TransformPoint(ir + x, -hw - iw - x, cw, -cw),
                TransformPoint(ir, -hw - iw, cw, -cw)
            };
            topLeft.Points = new PointCollection {
                TransformPoint(-ir, -hw, -cw, -cw),
                TransformPoint(-ir - x, 0, -cw, -cw),
                TransformPoint(-ir - width, -h, -cw, -cw),
                TransformPoint(-ir - width, -hw - iw - x + d, -cw, -cw),
                TransformPoint(-ir - x, -hw - iw - x, -cw, -cw),
                TransformPoint(-ir, -hw - iw, -cw, -cw)
            };

            bot.Points = new PointCollection {
                TransformPoint(ir, hw + iw, 0, ch),
                TransformPoint(ir + x, hw + iw + x, 0, ch),
                TransformPoint(ir + x - d, hw + iw + width, 0, ch),
                TransformPoint(-ir - x + d, hw + iw + width, 0, ch),
                TransformPoint(-ir - x, hw + iw + x, 0, ch),
                TransformPoint(-ir, hw + iw, 0, ch)
            };
            top.Points = new PointCollection {
                TransformPoint(ir, -hw - iw, 0, -ch),
                TransformPoint(ir + x, -hw - iw - x, 0, -ch),
                TransformPoint(ir + x - d, -hw - iw - width, 0, -ch),
                TransformPoint(-ir - x + d, -hw - iw - width, 0, -ch),
                TransformPoint(-ir - x, -hw - iw - x, 0, -ch),
                TransformPoint(-ir, -hw - iw, 0, -ch)
            };
        }
    }
}
