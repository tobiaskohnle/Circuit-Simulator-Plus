using System.Windows.Media;

namespace CircuitSimulatorPlus
{
    public interface ITheme
    {
        SolidColorBrush Background
        {
            get;
        }
        SolidColorBrush MainColor
        {
            get;
        }
        SolidColorBrush GridLine
        {
            get;
        }

        SolidColorBrush High
        {
            get;
        }
        SolidColorBrush Low
        {
            get;
        }

        SolidColorBrush LightHigh
        {
            get;
        }
        SolidColorBrush LightLow
        {
            get;
        }

        SolidColorBrush SegmentDisplayHigh
        {
            get;
        }
        SolidColorBrush SegmentDisplayLow
        {
            get;
        }

        SolidColorBrush SelectedHighlight
        {
            get;
        }
        SolidColorBrush SelectedHighlightFill
        {
            get;
        }

        SolidColorBrush SelectionRect
        {
            get;
        }
        SolidColorBrush SelectionRectFill
        {
            get;
        }

        SolidColorBrush FontColor
        {
            get;
        }

        SolidColorBrush Ticked
        {
            get;
        }
    }
}
