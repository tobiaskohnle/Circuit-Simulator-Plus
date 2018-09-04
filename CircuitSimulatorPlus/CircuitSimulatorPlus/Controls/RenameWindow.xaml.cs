using System;
using System.Windows;
using System.Windows.Input;

namespace CircuitSimulatorPlus.Controls
{
    public partial class RenameWindow : Window
    {
        public RenameWindow(string oldName)
        {
            InitializeComponent();

            InputTextBox.Text = oldName;
            InputTextBox.SelectAll();
            InputTextBox.Focus();
        }

        public new string Name
        {
            get;
            set;
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Name = InputTextBox.Text == "" ? null : InputTextBox.Text;
            Close();
        }

        void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
