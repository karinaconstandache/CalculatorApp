using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                MessageBox.Show($"Button {button.Content} clicked!");
            }
        }


        private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = (ComboBoxItem)e.AddedItems[0];

                if (selectedItem.Content.ToString() == "Programmer")
                {
                    // Open Programmer Mode Window
                    ProgrammerWindow programmerWindow = new ProgrammerWindow();
                    programmerWindow.Show();

                    // Close the Standard Calculator Window
                    this.Close();
                }
            }
        }
    }
}

