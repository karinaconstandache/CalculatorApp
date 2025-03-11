using System.Windows;

namespace CalculatorApp
{
    public partial class ProgrammerWindow : Window
    {
        public ProgrammerWindow()
        {
            InitializeComponent();
        }

        private void ReturnToStandardMode_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close Programmer Mode
        }
    }
}

