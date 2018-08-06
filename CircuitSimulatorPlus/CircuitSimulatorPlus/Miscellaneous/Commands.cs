using System.Windows.Input;

namespace CircuitSimulatorPlus
{
    public class Commands
    {
        public static RoutedUICommand New { get; } = new RoutedUICommand(
            "New Circuit", "New", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Open { get; } = new RoutedUICommand(
            "Open File", "Open", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Save { get; } = new RoutedUICommand(
            "Save File", "Save", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand SaveAs { get; } = new RoutedUICommand(
            "Save File As...", "SaveAs", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand Add { get; } = new RoutedUICommand(
            "Add Component", "Add", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand AddInput { get; } = new RoutedUICommand(
            "Add Empty Input", "AddInput", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand RemoveInput { get; } = new RoutedUICommand(
            " Empty Input", "RemoveInput", typeof(Commands),
            new InputGestureCollection
            {
               
            }
        );
        public static RoutedUICommand TrimInput { get; } = new RoutedUICommand(
            "Trim Empty Input", "TrimInput", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.M, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand CreateCable { get; } = new RoutedUICommand(
            "Create Cable", "CreateCable", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.E, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Split { get; } = new RoutedUICommand(
            "Split Cable Segment", "Split", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.P, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand InvertConnection { get; } = new RoutedUICommand(
            "Invert Connection", "InvertConnection", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemPeriod, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand ToggleButton { get; } = new RoutedUICommand(
            "Toggle Button", "ToggleButton", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand NextTick { get; } = new RoutedUICommand(
            "Next Tick", "NextTick", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Rename { get; } = new RoutedUICommand(
            "Rename", "Rename", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.R, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Align { get; } = new RoutedUICommand(
            "Align", "Align", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.L, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Copy { get; } = new RoutedUICommand(
            "Copy", "Copy", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Paste { get; } = new RoutedUICommand(
            "Paste", "Paste", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.V, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Cut { get; } = new RoutedUICommand(
            "Cut", "Cut", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Undo { get; } = new RoutedUICommand(
            "Undo", "Undo", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Z, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Redo { get; } = new RoutedUICommand(
            "Redo", "Redo", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Y, ModifierKeys.Control),
                new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand SelectAll { get; } = new RoutedUICommand(
            "Select All", "SelectAll", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.A, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Delete { get; } = new RoutedUICommand(
            "Delete", "Delete", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Delete),
                new KeyGesture(Key.Back)
            }
        );
        public static RoutedUICommand ZoomIn { get; } = new RoutedUICommand(
            "Zoom In", "ZoomIn", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemPlus, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand ZoomOut { get; } = new RoutedUICommand(
            "Zoom Out", "ZoomOut", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemMinus, ModifierKeys.Control)
            }
        );
    }
}
