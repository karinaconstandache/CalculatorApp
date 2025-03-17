// ProgrammerCalculatorViewModel.cs
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    public class ProgrammerCalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private int _currentValue = 0;
        private string _selectedOperator;
        private bool _isNewEntry = true;
        private int _selectedBase = 10; // Default to Decimal

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set { _displayText = value; OnPropertyChanged(nameof(DisplayText)); }
        }

        // The current numeric base (e.g., 2, 8, 10, 16)
        public int SelectedBase
        {
            get => _selectedBase;
            set { _selectedBase = value; OnPropertyChanged(nameof(SelectedBase)); }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }

        public ProgrammerCalculatorViewModel()
        {
            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
        }

        private void AppendNumber(string digit)
        {
            // (UI should already block disallowed digits.)
            if (_isNewEntry || DisplayText == "0")
                DisplayText = digit;
            else
                DisplayText += digit;
            _isNewEntry = false;
        }

        private void SetOperator(string op)
        {
            if (!_isNewEntry)
                CalculateResult();

            _currentValue = ParseDisplay();
            _selectedOperator = op;
            _isNewEntry = true;
        }

        private void CalculateResult()
        {
            if (_selectedOperator == null)
                return;

            int secondValue = ParseDisplay();
            int result = _currentValue;
            switch (_selectedOperator)
            {
                case "+": result += secondValue; break;
                case "-": result -= secondValue; break;
                case "*": result *= secondValue; break;
                case "/": result = secondValue != 0 ? result / secondValue : 0; break;
            }
            _currentValue = result;
            // Convert result to a string in the selected base.
            DisplayText = Convert.ToString(result, SelectedBase).ToUpper();
            _selectedOperator = null;
            _isNewEntry = true;
        }

        private int ParseDisplay()
        {
            try
            {
                return Convert.ToInt32(DisplayText, SelectedBase);
            }
            catch
            {
                return 0;
            }
        }

        private void Clear()
        {
            DisplayText = "0";
            _currentValue = 0;
            _selectedOperator = null;
            _isNewEntry = true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

