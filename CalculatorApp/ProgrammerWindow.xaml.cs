// ProgrammerWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CalculatorApp
{
    public partial class ProgrammerWindow : Window
    {
        private SolidColorBrush normalOperatorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A7A5F"));
        private SolidColorBrush hoverOperatorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E6652"));
        private SolidColorBrush normalDigitColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B8F71"));
        private SolidColorBrush hoverDigitColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A7A5F"));
        public ProgrammerWindow()
        {
            InitializeComponent();
        }

        private void ReturnToStandardMode_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close Programmer Mode window
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button button)
            {
                if ("0123456789ABCDEF".Contains(button.Content.ToString()))
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
                if ("0123456789ABCDEF".Contains(button.Content.ToString()))
                {
                    button.Background = normalDigitColor;
                }
                else
                {
                    button.Background = normalOperatorColor;
                }
            }
        }
    }
}
