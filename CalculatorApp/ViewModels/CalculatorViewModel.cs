using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CalculatorApp.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private string _historyText = "";
        private double _currentValue = 0;

        private string _selectedOperator;
        private bool _isNewEntry = true;

        private bool _isDigitGroupingEnabled = false;

        private string _clipboardText = string.Empty;

        private ObservableCollection<double> _memoryStack = new ObservableCollection<double>();
        private bool _isMemoryVisible = false;
        private double _currentMemoryValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (value.EndsWith("."))
                {
                    _displayText = value;
                }
                else if (double.TryParse(value.Replace(",", ""), out double numVal))
                {
                    _displayText = IsDigitGroupingEnabled
                        ? FormatDisplayValue(numVal.ToString())
                        : numVal.ToString();
                }
                else
                {
                    _displayText = value;
                }
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

        public bool IsDigitGroupingEnabled
        {
            get => _isDigitGroupingEnabled;
            set
            {
                if (_isDigitGroupingEnabled != value)
                {
                    _isDigitGroupingEnabled = value;
                    OnPropertyChanged(nameof(IsDigitGroupingEnabled));
                    RefreshDisplay();
                }
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

        public ICommand ToggleDigitGroupingCommand { get; }
        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand UnaryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand DecimalPointCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryItemClearCommand { get; }
        public ICommand ToggleMemoryCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }

        public CalculatorViewModel()
        {
            ToggleDigitGroupingCommand = new RelayCommand(_ => ToggleDigitGrouping());
            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            UnaryCommand = new RelayCommand(param => PerformUnaryOperation(param?.ToString() ?? ""));
            BackspaceCommand = new RelayCommand(_ => Backspace());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            DecimalPointCommand = new RelayCommand(_ => AppendDecimalPoint());

            MemoryAddCommand = new RelayCommand(param => AddToMemory(param as double?));
            MemorySubtractCommand = new RelayCommand(param => SubtractFromMemory(param as double?));
            MemoryStoreCommand = new RelayCommand(param => StoreInMemory(param as double?));
            MemoryClearCommand = new RelayCommand(param => ClearMemory());
            MemoryRecallCommand = new RelayCommand(param => RecallMemory(param as double?));
            MemoryItemClearCommand = new RelayCommand(param =>
            {
                if (param != null)
                {
                    double value;
                    if (param is double doubleParam)
                    {
                        value = doubleParam;
                    }
                    else if (double.TryParse(param.ToString(), out double parsedValue))
                    {
                        value = parsedValue;
                    }
                    else
                    {
                        return;
                    }

                    _memoryStack.Remove(value);
                    OnPropertyChanged(nameof(MemoryStack));
                }
            });
            ToggleMemoryCommand = new RelayCommand(_ => ToggleMemoryVisibility());

            CutCommand = new RelayCommand(ExecuteCut, CanExecuteCutOrCopy);
            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCutOrCopy);
            PasteCommand = new RelayCommand(ExecutePaste, CanExecutePaste);
        }

        private void ToggleDigitGrouping()
        {
            IsDigitGroupingEnabled = !IsDigitGroupingEnabled;
            RefreshDisplay();
        }

        private string FormatDisplayValue(string value)
        {
            if (!IsDigitGroupingEnabled || string.IsNullOrEmpty(value))
                return value;

            if (double.TryParse(value, out double numericValue))
            {
                if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
                    return value;

                if (value.Contains("."))
                {
                    string[] parts = value.Split('.');
                    string integerPart = string.Format("{0:N0}", double.Parse(parts[0]));
                    string decimalPart = parts.Length > 1 ? parts[1] : "";

                    return $"{integerPart}.{decimalPart}";
                }

                return string.Format("{0:N0}", numericValue);
            }

            return value;
        }

        private void RefreshDisplay()
        {
            string rawValue = DisplayText.Replace(",", "");

            if (double.TryParse(rawValue, out double numVal))
            {
                if (IsDigitGroupingEnabled)
                {
                    if (rawValue.Contains("."))
                    {
                        string[] parts = rawValue.Split('.');
                        string integerPart = string.Format("{0:N0}", double.Parse(parts[0]));
                        string decimalPart = parts.Length > 1 ? parts[1] : "";
                        _displayText = $"{integerPart}.{decimalPart}";
                    }
                    else
                    {
                        _displayText = string.Format("{0:N0}", numVal);
                    }
                }
                else
                {
                    _displayText = rawValue;
                }

                OnPropertyChanged(nameof(DisplayText));
            }
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

            if (IsDigitGroupingEnabled)
            {
                RefreshDisplay();
            }
        }

        private void AppendDecimalPoint()
        {
            if (_isNewEntry)
            {
                DisplayText = "0."; 
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
            HistoryText = "";
            _selectedOperator = null;
            _isNewEntry = true;

            if (IsDigitGroupingEnabled)
            {
                RefreshDisplay();
            }
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
                if (selectedMemoryValue.HasValue)
                {
                    int index = _memoryStack.IndexOf(selectedMemoryValue.Value);
                    if (index >= 0)
                    {
                        _memoryStack[index] += valueToAdd;
                    }
                }
                else
                {
                    if (_memoryStack.Any())
                    {
                        _memoryStack[_memoryStack.Count - 1] += valueToAdd;
                    }
                    else
                    {
                        _memoryStack.Add(valueToAdd);
                    }
                }
                OnPropertyChanged(nameof(MemoryStack));
            }
        }

        private void SubtractFromMemory(double? selectedMemoryValue = null)
        {
            if (double.TryParse(DisplayText, out double valueToSubtract))
            {
                if (selectedMemoryValue.HasValue)
                {
                    int index = _memoryStack.IndexOf(selectedMemoryValue.Value);
                    if (index >= 0)
                    {
                        _memoryStack[index] -= valueToSubtract;
                    }
                }
                else
                {
                    if (_memoryStack.Any())
                    {
                        _memoryStack[_memoryStack.Count - 1] -= valueToSubtract;
                    }
                    else
                    {
                        _memoryStack.Add(-valueToSubtract);
                    }
                }
                OnPropertyChanged(nameof(MemoryStack));
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

        private void ClearMemory(object selectedMemoryValue = null)
        {
            _memoryStack.Clear();
            OnPropertyChanged(nameof(MemoryStack));
        }

        private void RecallMemory(double? selectedMemoryValue = null)
        {
            if (selectedMemoryValue.HasValue)
            {
                DisplayText = selectedMemoryValue.Value.ToString();
            }
            else if (_memoryStack.Any())
            {
                DisplayText = _memoryStack.Last().ToString();
            }
            _isNewEntry = true;
        }

        private void ToggleMemoryVisibility()
        {
            IsMemoryVisible = !IsMemoryVisible; 
            OnPropertyChanged(nameof(IsMemoryVisible)); 
        }

        #region Cut / Copy / Paste Implementation

        private bool CanExecuteCutOrCopy(object parameter)
        {
            return !string.IsNullOrEmpty(DisplayText) && DisplayText != "0";
        }

        private bool CanExecutePaste(object parameter)
        {
            return !string.IsNullOrEmpty(_clipboardText);
        }

        private void ExecuteCopy(object parameter)
        {
            _clipboardText = DisplayText;
        }

        private void ExecuteCut(object parameter)
        {
            _clipboardText = DisplayText;
            DisplayText = "0";
            _isNewEntry = true;
        }

        private void ExecutePaste(object parameter)
        {
            if (_isNewEntry || DisplayText == "0")
            {
                DisplayText = _clipboardText;
            }
            else
            {
                DisplayText += _clipboardText;
            }
            _isNewEntry = false;
        }

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
