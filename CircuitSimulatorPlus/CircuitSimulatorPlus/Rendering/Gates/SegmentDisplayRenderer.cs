using System.Windows;
using System.Windows.Controls;
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

            Panel.SetZIndex(top, 4);
            Panel.SetZIndex(topLeft, 4);
            Panel.SetZIndex(topRight, 4);
            Panel.SetZIndex(center, 4);
            Panel.SetZIndex(botLeft, 4);
            Panel.SetZIndex(botRight, 4);
            Panel.SetZIndex(bot, 4);

            gate.OnRenderedChanged += OnRenderedChanged;

            if (gate.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (gate.IsRendered)
            {
                MainWindow.Self.canvas.Children.Add(top);
                MainWindow.Self.canvas.Children.Add(topLeft);
                MainWindow.Self.canvas.Children.Add(topRight);
                MainWindow.Self.canvas.Children.Add(center);
                MainWindow.Self.canvas.Children.Add(botLeft);
                MainWindow.Self.canvas.Children.Add(botRight);
                MainWindow.Self.canvas.Children.Add(bot);

                gate.OnSizeChanged += OnLayoutChanged;
                gate.OnPositionChanged += OnLayoutChanged;

                gate.Input[0].OnStateChanged += OnStateChanged;
                gate.Input[1].OnStateChanged += OnStateChanged;
                gate.Input[2].OnStateChanged += OnStateChanged;
                gate.Input[3].OnStateChanged += OnStateChanged;
                gate.Input[4].OnStateChanged += OnStateChanged;
                gate.Input[5].OnStateChanged += OnStateChanged;
                gate.Input[6].OnStateChanged += OnStateChanged;

                OnStateChanged();
                OnLayoutChanged();
            }
            else
            {
                MainWindow.Self.canvas.Children.Remove(top);
                MainWindow.Self.canvas.Children.Remove(topLeft);
                MainWindow.Self.canvas.Children.Remove(topRight);
                MainWindow.Self.canvas.Children.Remove(center);
                MainWindow.Self.canvas.Children.Remove(botLeft);
                MainWindow.Self.canvas.Children.Remove(botRight);
                MainWindow.Self.canvas.Children.Remove(bot);

                gate.OnSizeChanged -= OnLayoutChanged;
                gate.OnPositionChanged -= OnLayoutChanged;

                gate.Input[0].OnStateChanged -= OnStateChanged;
                gate.Input[1].OnStateChanged -= OnStateChanged;
                gate.Input[2].OnStateChanged -= OnStateChanged;
                gate.Input[3].OnStateChanged -= OnStateChanged;
                gate.Input[4].OnStateChanged -= OnStateChanged;
                gate.Input[5].OnStateChanged -= OnStateChanged;
                gate.Input[6].OnStateChanged -= OnStateChanged;
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
            top.Fill = gate.Input[0].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            topRight.Fill = gate.Input[1].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            botRight.Fill = gate.Input[2].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            bot.Fill = gate.Input[3].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            botLeft.Fill = gate.Input[4].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            topLeft.Fill = gate.Input[5].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
            center.Fill = gate.Input[6].State ? MainWindow.Self.Theme.SegmentDisplayHigh : MainWindow.Self.Theme.SegmentDisplayLow;
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
