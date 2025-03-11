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


        private SolidColorBrush normalColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A7A5F"));
        private SolidColorBrush hoverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E6652"));
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //to be implemented

        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = hoverColor;
            }
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = normalColor;
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

