using System.Windows.Input;

namespace CircuitSimulatorPlus
{
    class Commands
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
                new KeyGesture(Key.S, ModifierKeys.Control|ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand Add { get; } = new RoutedUICommand(
            "Add Component", "Add", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.N, ModifierKeys.Control|ModifierKeys.Shift)
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
                new KeyGesture(Key.Z, ModifierKeys.Control|ModifierKeys.Shift)
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
