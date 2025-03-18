using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    public class BaseOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }       
        public int BaseNumber { get; set; }  

        private string _convertedValue = "0";
        public string ConvertedValue
        {
            get => _convertedValue;
            set
            {
                if (_convertedValue != value)
                {
                    _convertedValue = value;
                    OnPropertyChanged(nameof(ConvertedValue));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string DisplayText => $"{Name} ({ConvertedValue})";

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ProgrammerCalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _displayText = "0";
        private int _currentValue = 0;
        private string _selectedOperator;
        private bool _isNewEntry = true;

        private int _selectedBase = 10;

        private ObservableCollection<BaseOption> _baseOptions;
        public ObservableCollection<BaseOption> BaseOptions
        {
            get => _baseOptions;
            set { _baseOptions = value; OnPropertyChanged(nameof(BaseOptions)); }
        }

        public ProgrammerCalculatorViewModel()
        {
            _baseOptions = new ObservableCollection<BaseOption>
            {
                new BaseOption { Name = "Binary",      BaseNumber = 2  },
                new BaseOption { Name = "Octal",       BaseNumber = 8  },
                new BaseOption { Name = "Decimal",     BaseNumber = 10 },
                new BaseOption { Name = "Hexadecimal", BaseNumber = 16 },
            };

            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());

            UpdateBaseOptions();
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged(nameof(DisplayText));
                    UpdateBaseOptions();
                }
            }
        }

        public int SelectedBase
        {
            get => _selectedBase;
            set
            {
                if (_selectedBase != value)
                {
                    int oldValue = ParseDisplay();

                    _selectedBase = value;
                    OnPropertyChanged(nameof(SelectedBase));
                    DisplayText = Convert.ToString(oldValue, _selectedBase).ToUpper();
                }
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }

        private void UpdateBaseOptions()
        {
            int currentValueDecimal = ParseDisplay();

            foreach (var opt in BaseOptions)
            {
                opt.ConvertedValue = Convert.ToString(currentValueDecimal, opt.BaseNumber).ToUpper();
            }
        }

        private void AppendNumber(string digit)
        {
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
                case "/": result = (secondValue != 0) ? result / secondValue : 0; break;
            }

            _currentValue = result;
            DisplayText = Convert.ToString(result, _selectedBase).ToUpper();

            _selectedOperator = null;
            _isNewEntry = true;
        }

        private void Clear()
        {
            DisplayText = "0";
            _currentValue = 0;
            _selectedOperator = null;
            _isNewEntry = true;
        }

        private int ParseDisplay()
        {
            try
            {
                return Convert.ToInt32(DisplayText, _selectedBase);
            }
            catch
            {
                return 0;
            }
        }

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
