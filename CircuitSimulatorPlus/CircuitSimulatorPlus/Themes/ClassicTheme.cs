using System.Windows.Media;

namespace CircuitSimulatorPlus
{
    public class ClassicTheme : ITheme
    {
        public SolidColorBrush Background
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 250, 250, 250));
            }
        }
        public SolidColorBrush MainColor
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 1, 3));
            }
        }
        public SolidColorBrush GridLine
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            }
        }

        public SolidColorBrush High
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(244, 255, 60, 45));
            }
        }
        public SolidColorBrush Low
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(244, 20, 20, 22));
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
                return new SolidColorBrush(Color.FromArgb(120, 187, 187, 190));
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
                return new SolidColorBrush(Color.FromArgb(70, 170, 170, 170));
            }
        }

        public SolidColorBrush SelectedHighlight
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 50, 150, 250));
            }
        }
        public SolidColorBrush SelectedHighlightFill
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(30, 16, 92, 255));
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
                return new SolidColorBrush(Color.FromArgb(255, 0, 1, 3));
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