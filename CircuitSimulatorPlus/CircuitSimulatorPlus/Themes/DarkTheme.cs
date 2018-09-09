using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CircuitSimulatorPlus
{
    public class DarkTheme : ITheme
    {
        public SolidColorBrush Background
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 4, 4, 8));
            }
        }
        public SolidColorBrush MainColor
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 210, 210, 210));
            }
        }
        public SolidColorBrush GridLine
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 20, 70, 105));
            }
        }

        public SolidColorBrush High
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(244, 255, 20, 42));
            }
        }
        public SolidColorBrush Low
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 210, 210, 210));
            }
        }

        public SolidColorBrush LightHigh
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(211, 255, 60, 45));
            }
        }
        public SolidColorBrush LightLow
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(60, 3, 3, 9));
            }
        }

        public SolidColorBrush SegmentDisplayHigh
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(211, 255, 55, 45));
            }
        }
        public SolidColorBrush SegmentDisplayLow
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(60, 3, 3, 9));
            }
        }

        public SolidColorBrush SelectedHighlight
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 50, 240, 250));
            }
        }
        public SolidColorBrush SelectedHighlightFill
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(30, 16, 200, 255));
            }
        }

        public SolidColorBrush FontColor
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public SolidColorBrush Ticked
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 250, 120, 30));
            }
        }
    }
}
