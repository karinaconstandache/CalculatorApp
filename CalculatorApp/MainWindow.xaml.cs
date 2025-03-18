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
using CalculatorApp.ViewModels;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {


        private SolidColorBrush normalOperatorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A7A5F"));
        private SolidColorBrush hoverOperatorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E6652"));
        private SolidColorBrush normalDigitColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B8F71"));
        private SolidColorBrush hoverDigitColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A7A5F"));
        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is CalculatorViewModel vm)
            {
                // Apply persisted digit grouping setting
                vm.IsDigitGroupingEnabled = App.Settings.IsDigitGroupingEnabled;
            }
            this.Closing += Window_Closing;
            this.KeyDown += MainWindow_KeyDown; // Attach KeyDown event handler
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is CalculatorViewModel viewModel)
            {
                // Prioritize operator keys first
                switch (e.Key)
                {
                    case Key.OemPlus:
                        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                            viewModel.OperatorCommand.Execute("+");
                        return;
                    case Key.OemMinus:
                    case Key.Subtract:
                        viewModel.OperatorCommand.Execute("-");
                        return;
                    case Key.Oem2:
                    case Key.Divide:
                        viewModel.OperatorCommand.Execute("/");
                        return;
                    case Key.Multiply:
                        viewModel.OperatorCommand.Execute("*");
                        return;
                    case Key.D8:
                        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                            viewModel.OperatorCommand.Execute("*");
                            return;
                        }
                        break;
                    case Key.Enter:
                        if (!string.IsNullOrEmpty(viewModel.DisplayText))
                            viewModel.EqualsCommand.Execute(null);
                        return;
                    case Key.OemPeriod:
                        viewModel.DecimalPointCommand.Execute(null);
                        return;
                    case Key.Escape:
                        viewModel.ClearCommand.Execute(null);
                        return;
                }

                // Handle digit keys separately
                if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    viewModel.NumberCommand.Execute((e.Key - Key.D0).ToString());
                }
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //to be implemented

        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button button)
            {
                if ("0123456789".Contains(button.Content.ToString()))
                {
                    button.Background = hoverDigitColor;
                }
                else
                {
                    button.Background = hoverOperatorColor;
                }
            }
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button button)
            {
                if ("0123456789".Contains(button.Content.ToString()))
                {
                    button.Background = normalDigitColor;
                }
                else
                {
                    button.Background = normalOperatorColor;
                }
            }
        }

        private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = (ComboBoxItem)e.AddedItems[0];

                if (selectedItem.Content.ToString() == "Programmer")
                {
                    ProgrammerWindow programmerWindow = new ProgrammerWindow();
                    programmerWindow.Show();

                    this.Close();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is CalculatorViewModel vm)
            {
                // Persist the digit grouping state
                App.Settings.IsDigitGroupingEnabled = vm.IsDigitGroupingEnabled;
                // Indicate that Standard mode was last used
                App.Settings.LastMode = "Standard";
            }
        }


    }
}

