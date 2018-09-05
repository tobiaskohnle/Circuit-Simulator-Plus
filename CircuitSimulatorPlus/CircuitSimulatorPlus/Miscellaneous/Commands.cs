using System.Windows.Input;

namespace CircuitSimulatorPlus
{
    public static class Commands
    {
        #region File
        public static RoutedUICommand New
        {
            get;
        } = new RoutedUICommand(
            "New", "New", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Open
        {
            get;
        } = new RoutedUICommand(
            "Open", "Open", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand OpenFolder
        {
            get;
        } = new RoutedUICommand(
            "Open Containing Folder", "OpenFolder", typeof(Commands)
        );
        public static RoutedUICommand Save
        {
            get;
        } = new RoutedUICommand(
            "Save", "Save", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand SaveAs
        {
            get;
        } = new RoutedUICommand(
            "Save As...", "SaveAs", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand Print
        {
            get;
        } = new RoutedUICommand(
            "Print", "Print", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.P, ModifierKeys.Control)
            }
        );
        #endregion

        #region Edit
        public static RoutedUICommand Undo
        {
            get;
        } = new RoutedUICommand(
            "Undo", "Undo", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Z, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Redo
        {
            get;
        } = new RoutedUICommand(
            "Redo", "Redo", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Y, ModifierKeys.Control),
                new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand Cut
        {
            get;
        } = new RoutedUICommand(
            "Cut", "Cut", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Copy
        {
            get;
        } = new RoutedUICommand(
            "Copy", "Copy", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Paste
        {
            get;
        } = new RoutedUICommand(
            "Paste", "Paste", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.V, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Delete
        {
            get;
        } = new RoutedUICommand(
            "Delete", "Delete", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Delete),
                new KeyGesture(Key.Back)
            }
        );
        public static RoutedUICommand SelectAll
        {
            get;
        } = new RoutedUICommand(
            "Select All", "SelectAll", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.A, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand DeselectAll
        {
            get;
        } = new RoutedUICommand(
            "Deselect All", "DeselectAll", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            }
        );
        #endregion

        #region View
        public static RoutedUICommand ResetView
        {
            get;
        } = new RoutedUICommand(
            "Reset View", "ResetView", typeof(Commands)
        );
        public static RoutedUICommand ZoomIn
        {
            get;
        } = new RoutedUICommand(
            "Zoom In", "ZoomIn", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemPlus, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand ZoomOut
        {
            get;
        } = new RoutedUICommand(
            "Zoom Out", "ZoomOut", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemMinus, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand ZoomSelection
        {
            get;
        } = new RoutedUICommand(
            "Zoom to Selection", "ZoomSelection", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Space, ModifierKeys.Control)
            }
        );
        #endregion

        #region Tools
        public static RoutedUICommand InvertConnection
        {
            get;
        } = new RoutedUICommand(
            "Invert Connection", "InvertConnection", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.I, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand Rename
        {
            get;
        } = new RoutedUICommand(
            "Rename", "Rename", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.R, ModifierKeys.Control)
            }
        );
        public static RoutedUICommand ToggleButton
        {
            get;
        } = new RoutedUICommand(
            "Toggle Button", "ToggleButton", typeof(Commands)
        );
        public static RoutedUICommand ToggleRisingEdge
        {
            get;
        } = new RoutedUICommand(
            "Toggle Rising Edge", "ToggleRisingEdge", typeof(Commands)
        );
        public static RoutedUICommand AddInput
        {
            get;
        } = new RoutedUICommand(
            "Add Empty Input", "AddInput", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        public static RoutedUICommand RemoveInput
        {
            get;
        } = new RoutedUICommand(
            "Remove Input", "RemoveInput", typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemMinus, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
        #endregion
    }
}
