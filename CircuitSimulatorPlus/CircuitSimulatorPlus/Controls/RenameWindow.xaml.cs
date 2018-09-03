using System;
using System.Windows;

namespace CircuitSimulatorPlus.Controls
{
    public partial class RenameWindow : Window
    {
        public RenameWindow()
        {
            InitializeComponent();

            InputTextBox.Focus();
        }

        public string Name
        {
            get;
            set;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Name = InputTextBox.Text;
            Close();
        }
    }
}
