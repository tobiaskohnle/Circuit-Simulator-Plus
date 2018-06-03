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
        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();

            lastCanvasClick = e.GetPosition(canvas);
            lastWindowClick = e.GetPosition(this);

            lastClickedObject = FindNearestObjectAt(lastCanvasClick);

            mouseMoved = false;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                movingScreen = true;
            }
            else if (lastClickedObject == null)
            {
                makingSelection = true;
            }
            else if (lastClickedObject.IsMovable)
            {
                movingObjects = true;
            }
            else
            {
                makingSelection = true;
            }

            if (makingSelection)
            {
                Canvas.SetLeft(selectVisual, lastWindowClick.X);
                Canvas.SetTop(selectVisual, lastWindowClick.Y);
                selectVisual.Visibility = Visibility.Visible;
            }
        }
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentWindowPos = e.GetPosition(this);
            Point currentCanvasPos = e.GetPosition(canvas);

            Vector windowMoved = currentWindowPos - lastWindowPos;
            Vector canvasMoved = e.GetPosition(canvas) - lastCanvasPos;

            if ((lastMousePos - lastWindowClick).LengthSquared >= MinDistMouseMoved * MinDistMouseMoved)
            {
                mouseMoved = true;
            }

            if (mouseMoved)
            {
                if (movingScreen)
                {
                    Matrix matrix = canvas.RenderTransform.Value;
                    matrix.Translate(windowMoved.X, windowMoved.Y);
                    canvas.RenderTransform = new MatrixTransform(matrix);
                }

                if (movingObjects)
                {
                    if (selectedObjects.Count == 0)
                    {
                        Select(lastClickedObject);
                    }
                    foreach (IClickable obj in selectedObjects)
                        if (obj is Gate)
                            (obj as Gate).Move(canvasMoved);
                }

                if (makingSelection)
                {
                    var innerRect = new Rect(lastCanvasClick, lastCanvasPos);
                    var outerRect = new Rect(lastCanvasClick, currentCanvasPos);
                    var innerObjects = clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(innerRect));
                    var outerObjects = clickableObjects.Where(obj => obj.Hitbox.IsIncludedIn(outerRect));

                    foreach (IClickable obj in outerObjects.Except(innerObjects))
                    {
                        if (ControlPressed)
                            SwitchSelected(obj);
                        else
                            Select(obj);
                    }

                    foreach (IClickable obj in innerObjects.Except(outerObjects))
                    {
                        if (ControlPressed)
                            SwitchSelected(obj);
                        else
                            Deselect(obj);
                    }

                    if (currentWindowPos.X < lastWindowClick.X)
                    {
                        Canvas.SetLeft(selectVisual, currentWindowPos.X);
                        selectVisual.Width = lastWindowClick.X - currentWindowPos.X;
                    }
                    else
                    {
                        selectVisual.Width = currentWindowPos.X - lastWindowClick.X;
                    }
                    if (currentWindowPos.Y < lastWindowClick.Y)
                    {
                        Canvas.SetTop(selectVisual, currentWindowPos.Y);
                        selectVisual.Height = lastWindowClick.Y - currentWindowPos.Y;
                    }
                    else
                    {
                        selectVisual.Height = currentWindowPos.Y - lastWindowClick.Y;
                    }
                }

                lastWindowPos = currentWindowPos;
                lastCanvasPos = e.GetPosition(canvas);
            }

            lastMousePos = currentWindowPos;

            UpdateGrid();
        }
        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            if (mouseMoved)
            {
                if (makingSelection)
                {
                    selectVisual.Width = selectVisual.Height = 0;
                    selectVisual.Visibility = Visibility.Collapsed;
                }

                if (movingObjects)
                {
                    Vector completeMove = lastCanvasPos - lastCanvasClick;
                    foreach (IClickable obj in selectedObjects)
                    {
                        if (obj is Gate)
                            (obj as Gate).Move(-completeMove);
                    }

                    if (completeMove.LengthSquared > 0.5 * 0.5)
                    {
                        PerformAction(new MoveObjectAction(selectedObjects,
                            new Vector(Math.Round(completeMove.X), Math.Round(completeMove.Y))));
                    }
                }
            }
            else
            {
                if (lastClickedObject == null)
                {
                    DeselectAll();
                }
                else
                {
                    //if (ControlPressed == false)
                    //{
                    //    DeselectAll();
                    //}
                    bool connectionCreated = false;
                    if (lastClickedObject is ConnectionNode)
                    {
                        foreach (IClickable obj in selectedObjects)
                        {
                            if (obj is InputNode
                                && lastClickedObject is OutputNode
                                && (obj as ConnectionNode).IsEmpty)
                            {
                                (lastClickedObject as OutputNode).ConnectTo(obj as InputNode);
                                connectionCreated = true;
                            }
                            else if (lastClickedObject is InputNode
                                && obj is OutputNode
                                && (lastClickedObject as ConnectionNode).IsEmpty)
                            {
                                (obj as OutputNode).ConnectTo(lastClickedObject as InputNode);
                                connectionCreated = true;
                            }
                        }
                        if (connectionCreated)
                        {
                            if (ControlPressed)
                                Deselect(lastClickedObject);
                            else
                                DeselectAll();
                        }
                    }

                    if (connectionCreated == false)
                    {
                        SwitchSelected(lastClickedObject);
                    }
                }
            }

            //if (drawingCable)
            //{
            //    Cable lastcable = cables.Last();
            //    Point pos = e.GetPosition(canvas);
            //    pos.X = Math.Round(pos.X);
            //    pos.Y = Math.Round(pos.Y);
            //    lastcable.AddPoint(pos);
            //}

            //if (drawingCable && lastClickedObject is ConnectionNode)
            //{
            //    drawingCable = false;
            //}

            makingSelection = false;
            movingObjects = false;
            movingScreen = false;
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point currentPos = e.GetPosition(canvas);
            double scale = e.Delta > 0 ? 1 / ScaleFactor : ScaleFactor;

            Matrix matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(scale, scale, currentPos.X, currentPos.Y);
            canvas.RenderTransform = new MatrixTransform(matrix);
            currentScale *= scale;
            UpdateGrid();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && singleTicks)
                TickQueue();
        }
        void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        void Window_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = mouseMoved;
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        void NewFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in contextGate.Context)
                gate.Renderer.Unrender();
            foreach (Cable cable in cables)
                cable.Renderer.Unrender();
        }
        void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            NewFile_Click(sender, e);
            var dialog = new OpenFileDialog();
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                contextGate = StorageConverter.ToGate(Storage.Load(dialog.FileName));
                UpdateClickableObjects();
                foreach (Gate gate in contextGate.Context)
                {
                    gate.Renderer = new GateRenderer(canvas, gate, OnGateInputClicked, OnGateOutputClicked);
                    gate.Renderer.Render();
                    for (int i = 0; i < gate.Output.Count; i++)
                    {
                        OutputNode node = gate.Output[i];
                        Point p1 = new Point();
                        p1.X = gate.Position.X + 3 + 1;
                        p1.Y = gate.Position.Y + (double)4 * (1 + 2 * i) / (2 * gate.Output.Count);
                        foreach (InputNode inNode in node.NextConnectedTo)
                        {
                            Gate inGate = inNode.Owner;
                            int index = 0;
                            for (int j = 0; j < inGate.Input.Count; j++)
                            {
                                if (inGate.Input[j] == inNode)
                                {
                                    index = j;
                                    break;
                                }
                            }
                            Point p2 = new Point();
                            p2.X = inGate.Position.X - 1;
                            p2.Y = inGate.Position.Y + (double)4 * (1 + 2 * index) / (2 * inGate.Input.Count);
                            Cable cable = new Cable();
                            cable.Output = node;
                            cable.Input = inNode;
                            cable.Renderer = new CableRenderer(canvas, cable);
                            cable.AddPoint(p1, true);
                            cable.AddPoint(p2, true);
                            cables.Add(cable);
                        }
                    }
                }
            }
        }
        void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilePath != null && !saved)
                Storage.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
            else
                SaveFileAs_Click(sender, e);
            saved = true;
            UpdateTitle();
        }
        void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = fileName;
            dialog.DefaultExt = FileExtention;
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                fileName = dialog.SafeFileName;
                Storage.Save(currentFilePath, StorageConverter.ToStorageObject(contextGate));
                saved = true;
                UpdateTitle();
            }
        }

        void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUndo)
            {
                Action lastAction = undoStack.Pop();
                lastAction.Undo();
                redoStack.Push(lastAction);
            }
        }
        void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (AllowRedo)
            {
                Action lastAction = redoStack.Pop();
                lastAction.Redo();
                undoStack.Push(lastAction);
            }
        }
        void Copy_Click(object sender, RoutedEventArgs e)
        {

        }
        void Cut_Click(object sender, RoutedEventArgs e)
        {

        }
        void Paste_Click(object sender, RoutedEventArgs e)
        {

        }
        void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnySelected)
            {
                //PerformAction(new DeleteGateAction(contextGate, selected));
                DeleteSelected();
            }
        }

        void DefaultView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }
        void ZoomIn_Click(object sender, RoutedEventArgs e)
        {

        }
        void ZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }

        void NewGate_Click(object sender, RoutedEventArgs e)
        {

        }
        void Rename_Click(object sender, RoutedEventArgs e)
        {

        }
        void ResizeGate_Click(object sender, RoutedEventArgs e)
        {

        }
        void EmptyInput_Click(object sender, RoutedEventArgs e)
        {

        }
        void TrimInput_Click(object sender, RoutedEventArgs e)
        {

        }
        void InvertConnection_Click(object sender, RoutedEventArgs e)
        {
            foreach (IClickable obj in selectedObjects)
                if (obj is ConnectionNode)
                {
                    ConnectionNode connectionNode = obj as ConnectionNode;
                    connectionNode.Invert();
                    Tick(connectionNode);
                }
        }
        void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }
        void ContextGate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = FileFilter;
            if (dialog.ShowDialog() == true)
            {
                Gate gate = StorageConverter.ToGate(Storage.Load(dialog.FileName));
                gate.Position = new Point(Math.Round(lastCanvasClick.X), Math.Round(lastCanvasClick.Y));
                gate.Renderer = new GateRenderer(canvas, gate);
                gate.Renderer.Render();
                Add(gate);
                Select(gate);
            }
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
        }

        void SingleTicks_Click(object sender, RoutedEventArgs e)
        {
            singleTicks = !singleTicks;
            MessageBox.Show($"Single Ticks {(singleTicks ? "Enabled (Advance by pressing 'T')" : "Disabled")}");
            if (singleTicks == false && tickedNodes.Count > 0)
                timer.Start();
        }
        void TickAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Gate gate in contextGate.Context)
            {
                foreach (InputNode inputNode in gate.Input)
                    Tick(inputNode);
                foreach (OutputNode outputNode in gate.Output)
                    Tick(outputNode);
            }
        }

        void OnGateOutputClicked(object sender, EventArgs e)
        {
            //Gate gate = (Gate)sender;
            //int index = ((IndexEventArgs)e).Index;

            //if (!drawingCable)
            //{
            //    Point point = new Point();
            //    point.X = gate.Position.X + 3 + 1;
            //    point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Output.Count);
            //    Cable cable = new Cable();
            //    cables.Add(cable);
            //    cable.Renderer = new CableRenderer(canvas, cable);
            //    cable.Output = gate.Output[index];
            //    drawingCable = true;
            //}
        }
        void OnGateInputClicked(object sender, EventArgs e)
        {
            //Gate gate = (Gate)sender;
            //int index = ((IndexEventArgs)e).Index;

            //if (drawingCable)
            //{
            //    Point point = new Point();
            //    point.X = gate.Position.X - 1;
            //    point.Y = gate.Position.Y + (double)4 * (1 + 2 * index) / (2 * gate.Input.Count);
            //    Cable lastcable = cables.Last();
            //    lastcable.AddPoint(point, true);
            //    lastcable.Input = gate.Input[index];
            //    lastcable.Output.ConnectTo(lastcable.Input);
            //    Tick(lastcable.Input);
            //    drawingCable = false;
            //}
        }

        void Allow_Copy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Allow_Paste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DataOnClipboard;
        }
        void Allow_Undo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AllowUndo;
        }
        void Allow_Redo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AllowRedo;
        }
        void Allow_Cut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Allow_Delete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnySelected;
        }
        void Allow_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !saved;
        }
        void Allow_SaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !saved;
        }
        void Allow_Rename(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyGateSelected;
        }
        void Allow_EmptyInput(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyGateSelected;
        }
        void Allow_TrimInput(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyGateSelected;
        }
        void Allow_InvertConnection(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AnyConnectionSelected;
        }
    }
}
