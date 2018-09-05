using System;
using System.Globalization;
using System.Text.RegularExpressions;
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
            if (InputTextBox.Text == "")
            {
                Name = null;
            }
            else
            {
                Name = Regex.Replace(
                    InputTextBox.Text,
                    @"\\u(?<hex>[a-zA-Z0-9]{4})",
                    match =>
                    {
                        return ((char)Int32.Parse(
                            match.Groups["hex"].Value,
                            NumberStyles.HexNumber)
                        ).ToString();
                    }
                );
            }
            Close();
        }
    }
}
