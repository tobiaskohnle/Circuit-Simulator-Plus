using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CircuitSimulatorPlus
{
    class DarkFantasticTheme :ITheme
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
                return new SolidColorBrush(Color.FromArgb(244, 0, 255, 0));
            }
        }
        public SolidColorBrush Low
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255,255,193,37));
            }
        }

        public SolidColorBrush LightHigh
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(211, 0, 255, 0));
            }
        }
        public SolidColorBrush LightLow
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(0, 0, 139, 0));
            }
        }

        public SolidColorBrush SegmentDisplayHigh
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(211, 0, 255, 0));
            }
        }
        public SolidColorBrush SegmentDisplayLow
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(60, 255, 255, 0));
            }
        }

        public SolidColorBrush SelectedHighlight
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 32,178,170));
            }
        }
        public SolidColorBrush SelectedHighlightFill
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(30, 32, 178, 170));
            }
        }

        public SolidColorBrush SelectionRect
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(200, 50, 150, 250));
            }
        }
        public SolidColorBrush SelectionRectFill
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(63, 0, 120, 215));
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
                return new SolidColorBrush(Color.FromArgb(255, 139,0,0));
            }
        }
    }
}
