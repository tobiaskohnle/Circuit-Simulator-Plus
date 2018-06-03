using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace CircuitSimulatorPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            canvas.SnapsToDevicePixels = true;

            RenderOptions.SetEdgeMode(backgoundLayerCanvas, EdgeMode.Aliased);
            backgoundLayerCanvas.SnapsToDevicePixels = true;

            UpdateTitle();
            ResetView();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                contextGate = StorageConverter.ToGate(Storage.Load(args[1]));
            else
                contextGate = new Gate();
            foreach (Gate gate in contextGate.Context)
                gate.Renderer.Render();

            UpdateClickableObjects();

            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += TimerTick;
            DrawGrid();
        }

        #region Constants
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileExtention;
        public const string DefaultTitle = "untitled";
        public const string FileExtention = ".tici";
        public const double MinDistMouseMoved = 5;
        public const double DefaultGridSize = 20;
        public const double ScaleFactor = 0.9;
        public const double LineWidth = 0.1;
        public const int UndoBufferSize = 32;
        #endregion

        #region Properties
        Point lastMousePos;

        Point lastWindowPos;
        Point lastWindowClick;

        Point lastCanvasPos;
        Point lastCanvasClick;

        bool makingSelection;
        bool movingObjects;
        bool movingScreen;
        bool mouseMoved;

        bool saved = true;
        bool singleTicks;

        string fileName = DefaultTitle;
        string currentFilePath;

        Queue<ConnectionNode> tickedNodes = new Queue<ConnectionNode>();
        DispatcherTimer timer = new DispatcherTimer();

        List<IClickable> clickableObjects = new List<IClickable>();
        List<IClickable> selectedObjects = new List<IClickable>();

        Stack<Action> undoStack = new Stack<Action>();
        Stack<Action> redoStack = new Stack<Action>();

        List<Cable> cables = new List<Cable>();
        Gate contextGate;
        IClickable lastClickedObject;

        Pen backgroundGridPen;
        double currentScale;
        #endregion

        #region Gates
        public void Add(Gate gate)
        {
            contextGate.Context.Add(gate);
            clickableObjects.Add(gate);
            foreach (InputNode input in gate.Input)
                Add(input);
            foreach (OutputNode output in gate.Output)
                Add(output);
        }
        public void Add(ConnectionNode connectionNode)
        {
            clickableObjects.Add(connectionNode);
        }
        public void CreateGate(Gate gate, int amtInputs, int amtOutputs, Point at)
        {
            contextGate.Context.Add(gate);
            gate.Renderer = new GateRenderer(canvas, gate, OnGateInputClicked, OnGateOutputClicked);

            for (int i = 0; i < amtInputs; i++)
                gate.Input.Add(new InputNode(gate));
            for (int i = 0; i < amtOutputs; i++)
                gate.Output.Add(new OutputNode(gate));

            gate.Position = new Point(Math.Round(at.X), Math.Round(at.Y));
            gate.Renderer.Render();

            //PerformAction(new CreateGateAction(contextGate, gate));

            Select(gate);
            Add(gate);
        }

        public void Tick(ConnectionNode node)
        {
            tickedNodes.Enqueue(node);
            if (singleTicks == false)
                timer.Start();
        }
        public void TickQueue()
        {
            List<ConnectionNode> copy = tickedNodes.ToList();
            tickedNodes.Clear();
            foreach (ConnectionNode ticked in copy)
                ticked.Tick(tickedNodes);
            if (tickedNodes.Count == 0)
                timer.Stop();
        }
        public void TimerTick(object sender, EventArgs e)
        {
            TickQueue();
        }

        public void Select(IClickable obj)
        {
            if (!obj.IsSelected)
            {
                selectedObjects.Add(obj);
                obj.IsSelected = true;
            }
        }
        public void Deselect(IClickable obj)
        {
            if (obj.IsSelected)
            {
                selectedObjects.Remove(obj);
                obj.IsSelected = false;
            }
        }
        public void SwitchSelected(IClickable obj)
        {
            if (obj.IsSelected)
            {
                Deselect(obj);
            }
            else
            {
                Select(obj);
            }
        }
        public IEnumerable<IClickable> GetObjectsIn(Rect rect)
        {
            return clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(rect));
        }
        public void SelectAllIn(Rect rect)
        {
            foreach (IClickable obj in GetObjectsIn(rect))
                Select(obj);
        }
        public void SwitchSelectionIn(Rect rect)
        {
            foreach (IClickable obj in GetObjectsIn(rect))
                SwitchSelected(obj);
        }
        public void SelectAll()
        {
            foreach (IClickable obj in clickableObjects)
                Select(obj);
        }
        public void DeselectAll()
        {
            foreach (IClickable obj in selectedObjects)
                obj.IsSelected = false;
            selectedObjects.Clear();
        }

        public void DeleteSelected()
        {
            foreach (IClickable obj in selectedObjects)
            {
                if (obj is Gate)
                    Delete(obj as Gate);
                else if (obj is ConnectionNode)
                    (obj as ConnectionNode).Clear();
            }
            DeselectAll();
        }
        public void Delete(Gate gate)
        {
            foreach (InputNode input in gate.Input)
                input.Clear();
            foreach (OutputNode output in gate.Output)
                output.Clear();
            gate.Renderer.Unrender();
            contextGate.Context.Remove(gate);
        }

        public IClickable FindNearestObjectAt(Point pos)
        {
            IClickable nearest = null;
            double best = Double.PositiveInfinity;

            foreach (IClickable obj in clickableObjects.Where(obj => obj.Hitbox.IncludesPos(pos)))
            {
                double dist = obj.Hitbox.DistanceTo(pos);
                if (dist < best)
                {
                    nearest = obj;
                    best = dist;
                }
            }

            return nearest;
        }

        public void CreateAndGate(object sender, RoutedEventArgs e)
        {
            CreateGate(new Gate(Gate.GateType.And), 2, 1, lastCanvasClick);
        }
        public void CreateOrGate(object sender, RoutedEventArgs e)
        {
            CreateGate(new Gate(Gate.GateType.Or), 2, 1, lastCanvasClick);
        }
        public void CreateNotGate(object sender, RoutedEventArgs e)
        {
            var newGate = new Gate(Gate.GateType.Identity);
            CreateGate(newGate, 1, 1, lastCanvasClick);
            newGate.Output[0].Invert();
            Tick(newGate.Output[0]);
        }
        #endregion

        #region Visuals
        public void ResetView()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Scale(DefaultGridSize, DefaultGridSize);
            canvas.RenderTransform = new MatrixTransform(matrix);
            currentScale = 1;
        }
        public void UpdateTitle()
        {
            Title = $"{fileName}{(saved ? "" : " \u2022")} - {WindowTitle}";
        }
        public void DrawGrid()
        {
            double linewidth = LineWidth / currentScale;
            backgroundGridPen = new Pen(Brushes.LightGray, linewidth / 2);
            var geometry = new RectangleGeometry(new Rect(0, 0, 1, 1));
            var drawing = new GeometryDrawing(Brushes.White, backgroundGridPen, geometry);
            var brush = new DrawingBrush
            {
                Drawing = drawing,
                Viewport = new Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Stretch = Stretch.Fill
            };
            backgoundLayerCanvas.Background = brush;
            UpdateGrid();
        }
        public void UpdateGrid()
        {
            backgroundGridPen.Thickness = LineWidth / currentScale / 2;
            backgoundLayerCanvas.Background.Transform = canvas.RenderTransform;
        }
        #endregion

        #region Misc
        public void PerformAction(Action action)
        {
            saved = false;
            UpdateTitle();
            action.Redo();
            undoStack.Push(action);
        }
        public bool SavePrompt()
        {
            //if (!saved)
            //{
            //    switch (MessageBox.Show(
            //        "Would you like to save your changes?",
            //        "Save?",
            //        MessageBoxButton.YesNoCancel
            //    ))
            //    {
            //    case MessageBoxResult.Yes:
            //        Save(); return saved;
            //    case MessageBoxResult.No:
            //        return true;
            //    case MessageBoxResult.Cancel:
            //        return false;
            //    }
            //}
            return true;
        }
        public void UpdateClickableObjects()
        {
            clickableObjects.Clear();
            foreach (Gate gate in contextGate.Context)
            {
                clickableObjects.Add(gate);
                foreach (InputNode input in gate.Input)
                    clickableObjects.Add(input);
                foreach (OutputNode output in gate.Output)
                    clickableObjects.Add(output);
            }
        }
        #endregion

        #region Util
        public bool AllowUndo
        {
            get {
                return undoStack.Count > 0;
            }
        }
        public bool AllowRedo
        {
            get {
                return redoStack.Count > 0;
            }
        }
        public bool AnySelected
        {
            get {
                return selectedObjects.Count > 0;
            }
        }
        public bool AnyGateSelected
        {
            get {
                return selectedObjects.Exists(obj => obj is Gate);
            }
        }
        public bool AnyConnectionSelected
        {
            get {
                return selectedObjects.Exists(obj => obj is ConnectionNode);
            }
        }
        public bool ControlPressed
        {
            get {
                return (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            }
        }
        public bool DataOnClipboard
        {
            get {
                return Clipboard.ContainsData(FileExtention);
            }
        }
        #endregion
    }
}
