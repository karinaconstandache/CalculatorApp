using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private double _currentValue = 0;
        private string _selectedOperator;
        private bool _isNewEntry = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }

        public CalculatorViewModel()
        {
            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
        }

        private void AppendNumber(string number)
        {
            if (_isNewEntry || DisplayText == "0")
            {
                DisplayText = number; // Replace 0 instead of appending
            }
            else
            {
                DisplayText += number; // Append numbers normally
            }
            _isNewEntry = false;
        }


        private void SetOperator(string op)
        {
            if (!_isNewEntry) // If an operator is pressed after a number
            {
                CalculateResult(); // Compute the previous operation before setting a new one
            }

            _currentValue = double.Parse(DisplayText);
            _selectedOperator = op;
            _isNewEntry = true;
        }


        private void CalculateResult()
        {
            if (_selectedOperator == null)
                return; // Prevents error when "=" is pressed without an operator

            double secondValue = double.Parse(DisplayText);
            switch (_selectedOperator)
            {
                case "+":
                    DisplayText = (_currentValue + secondValue).ToString();
                    break;
                case "-":
                    DisplayText = (_currentValue - secondValue).ToString();
                    break;
                case "*":
                    DisplayText = (_currentValue * secondValue).ToString();
                    break;
                case "/":
                    DisplayText = secondValue != 0 ? (_currentValue / secondValue).ToString() : "Error";
                    break;
                case "%":
                    DisplayText = (_currentValue % secondValue).ToString();
                    break;
                case "sqrt":
                    DisplayText = Math.Sqrt(_currentValue).ToString();
                    break;
                case "^2":
                    DisplayText = (_currentValue * _currentValue).ToString();
                    break;
                case "+/-":
                    DisplayText = (-_currentValue).ToString();
                    break;
                case "1/x":
                    DisplayText = _currentValue != 0 ? (1 / _currentValue).ToString() : "Error";
                    break;
            }
            _selectedOperator = null; // Reset operator after calculation
            _isNewEntry = true;
        }


        private void Clear()
        {
            DisplayText = "0";
            _currentValue = 0;
            _selectedOperator = null;
            _isNewEntry = true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
