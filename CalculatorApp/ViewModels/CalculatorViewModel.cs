using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace CalculatorApp.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private string _historyText = "";
        private double _currentValue = 0;
        private string _selectedOperator;
        private bool _isNewEntry = true;

        private ObservableCollection<double> _memoryStack = new ObservableCollection<double>();
        private bool _isMemoryVisible = false;
        private double _currentMemoryValue;

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

        public string HistoryText
        {
            get => _historyText;
            set
            {
                _historyText = value;
                OnPropertyChanged(nameof(HistoryText));
            }
        }

        public ObservableCollection<double> MemoryStack
        {
            get => _memoryStack;
            set
            {
                _memoryStack = value;
                OnPropertyChanged(nameof(MemoryStack));
            }
        }

        public bool IsMemoryVisible
        {
            get => _isMemoryVisible;
            set
            {
                _isMemoryVisible = value;
                OnPropertyChanged(nameof(IsMemoryVisible));
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand UnaryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand DecimalPointCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemoryRemoveCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand ToggleMemoryCommand { get; }

        public CalculatorViewModel()
        {
            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            UnaryCommand = new RelayCommand(param => PerformUnaryOperation(param?.ToString() ?? ""));
            BackspaceCommand = new RelayCommand(_ => Backspace());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            DecimalPointCommand = new RelayCommand(_ => AppendDecimalPoint());

            MemoryAddCommand = new RelayCommand(param => AddToMemory(param as double?));
            MemoryRemoveCommand = new RelayCommand(param => RemoveFromMemory(param as double?));
            MemoryStoreCommand = new RelayCommand(param => StoreInMemory(param as double?));
            MemoryClearCommand = new RelayCommand(param => ClearMemory());
            ToggleMemoryCommand = new RelayCommand(_ => ToggleMemoryVisibility());
        }

        private void AppendNumber(string number)
        {
            if (_isNewEntry || DisplayText == "0")
            {
                DisplayText = number;
            }
            else
            {
                DisplayText += number;
            }
            _isNewEntry = false;
        }

        private void AppendDecimalPoint()
        {
            if (_isNewEntry)
            {
                DisplayText = "0.";  // Start a new decimal number if necessary
                _isNewEntry = false;
            }
            else if (!DisplayText.Contains("."))
            {
                DisplayText += ".";
            }
        }

        private void SetOperator(string op)
        {
            if (!_isNewEntry)
            {
                CalculateResult();
            }

            _currentValue = double.Parse(DisplayText);
            _selectedOperator = op;
            HistoryText = $"{_currentValue} {op}";
            _isNewEntry = true;
        }

        private void CalculateResult()
        {
            if (_selectedOperator == null)
                return;

            double secondValue = double.Parse(DisplayText);
            double result = _currentValue;

            switch (_selectedOperator)
            {
                case "+":
                    result += secondValue;
                    break;
                case "-":
                    result -= secondValue;
                    break;
                case "*":
                    result *= secondValue;
                    break;
                case "/":
                    result = secondValue != 0 ? result / secondValue : double.NaN;
                    break;
                case "%":
                    result = secondValue != 0 ? result % secondValue : double.NaN;
                    break;
            }

            DisplayText = result.ToString();
            _currentValue = result;
            HistoryText = ""; // Clear history after calculation
            _selectedOperator = null;
            _isNewEntry = true;
        }

        private void PerformUnaryOperation(string op)
        {
            double value = double.Parse(DisplayText);
            double result = value;

            switch (op)
            {
                case "sqrt":
                    result = Math.Sqrt(value);
                    break;
                case "^2":
                    result = value * value;
                    break;
                case "+/-":
                    result = -value;
                    break;
                case "1/x":
                    result = value != 0 ? 1 / value : double.NaN;
                    break;
            }

            // If there is a pending operator, update DisplayText but keep _currentValue
            if (_selectedOperator != null)
            {
                DisplayText = result.ToString();
            }
            else
            {
                _currentValue = result;
                DisplayText = _currentValue.ToString();
            }

            _isNewEntry = true;
        }

        private void Backspace()
        {
            if (_isNewEntry)
            {
                return;
            }

            if (DisplayText.Length > 1)
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }
            else
            {
                DisplayText = "0";
                _isNewEntry = true;
            }
        }

        private void Clear()
        {
            DisplayText = "0";
            HistoryText = "";
            _currentValue = 0;
            _selectedOperator = null;
            _isNewEntry = true;
        }

        private void ClearEntry()
        {
            DisplayText = "0";
        }
        private void AddToMemory(double? selectedMemoryValue = null)
        {
            if (double.TryParse(DisplayText, out double valueToAdd))
            {
                if (selectedMemoryValue.HasValue) // Called from M> menu (specific value)
                {
                    int index = _memoryStack.IndexOf(selectedMemoryValue.Value);
                    if (index >= 0)
                    {
                        _memoryStack[index] += valueToAdd;
                    }
                }
                else // Called from the main window (modify the last memory value)
                {
                    if (_memoryStack.Any())
                    {
                        _memoryStack[_memoryStack.Count - 1] += valueToAdd;
                    }
                    else
                    {
                        _memoryStack.Add(valueToAdd); // If empty, initialize memory with the value
                    }
                }
                OnPropertyChanged(nameof(MemoryStack)); // Notify UI update
            }
        }

        private void RemoveFromMemory(double? selectedMemoryValue = null)
        {
            if (double.TryParse(DisplayText, out double valueToSubtract))
            {
                if (selectedMemoryValue.HasValue) // Called from M> menu (specific value)
                {
                    int index = _memoryStack.IndexOf(selectedMemoryValue.Value);
                    if (index >= 0)
                    {
                        _memoryStack[index] -= valueToSubtract;
                    }
                }
                else // Called from the main window (modify the last memory value)
                {
                    if (_memoryStack.Any())
                    {
                        _memoryStack[_memoryStack.Count - 1] -= valueToSubtract;
                    }
                    else
                    {
                        _memoryStack.Add(-valueToSubtract); // If empty, initialize memory with the value
                    }
                }
                OnPropertyChanged(nameof(MemoryStack)); // Notify UI update
            }
        }

        private void StoreInMemory(double? selectedMemoryValue)
        {
            if (double.TryParse(DisplayText, out double value))
            {
                _memoryStack.Add(value);
                OnPropertyChanged(nameof(MemoryStack));
            }
        }

        private void ClearMemory()
        {
            _memoryStack.Clear();  // Clear the entire memory stack
            OnPropertyChanged(nameof(MemoryStack));  // Notify that the memory stack has been updated
        }
        private void ToggleMemoryVisibility()
        {
            IsMemoryVisible = !IsMemoryVisible;  // Toggle the visibility state
            OnPropertyChanged(nameof(IsMemoryVisible));  // Notify the UI to update
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
