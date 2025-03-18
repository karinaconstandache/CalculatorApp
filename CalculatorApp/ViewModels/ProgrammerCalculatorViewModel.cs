using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    // Simple helper class to hold each base’s name, numeric base, and the converted value
    public class BaseOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }        // e.g. "Binary", "Decimal", ...
        public int BaseNumber { get; set; }     // e.g. 2, 8, 10, 16

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
                    OnPropertyChanged(nameof(DisplayText)); // So the combo box text updates
                }
            }
        }

        // What appears in the ComboBox (e.g. "Binary (1010)")
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

        // We’ll store the *actual* numeric base the user is working in
        private int _selectedBase = 10; // default to Decimal

        // Expose the base options in an ObservableCollection
        private ObservableCollection<BaseOption> _baseOptions;
        public ObservableCollection<BaseOption> BaseOptions
        {
            get => _baseOptions;
            set { _baseOptions = value; OnPropertyChanged(nameof(BaseOptions)); }
        }

        public ProgrammerCalculatorViewModel()
        {
            // Initialize the base options
            _baseOptions = new ObservableCollection<BaseOption>
            {
                new BaseOption { Name = "Binary",      BaseNumber = 2  },
                new BaseOption { Name = "Octal",       BaseNumber = 8  },
                new BaseOption { Name = "Decimal",     BaseNumber = 10 },
                new BaseOption { Name = "Hexadecimal", BaseNumber = 16 },
            };

            // Initialize commands
            NumberCommand = new RelayCommand(param => AppendNumber(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());

            // Make sure the combo box’s display is correct on startup
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
                    // Whenever DisplayText changes, recalc the base options
                    UpdateBaseOptions();
                }
            }
        }

        // The numeric base the user selected (2, 8, 10, or 16)
        public int SelectedBase
        {
            get => _selectedBase;
            set
            {
                if (_selectedBase != value)
                {
                    // 1) Parse the old display in the old base
                    int oldValue = ParseDisplay();

                    // 2) Update the selected base
                    _selectedBase = value;
                    OnPropertyChanged(nameof(SelectedBase));

                    // 3) Convert oldValue to the new base and store in DisplayText
                    DisplayText = Convert.ToString(oldValue, _selectedBase).ToUpper();
                }
            }
        }

        // Commands
        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }

        // Called each time DisplayText changes or user changes the base
        private void UpdateBaseOptions()
        {
            // Parse the current display as an integer in the *current* base
            int currentValueDecimal = ParseDisplay();

            // For each base in the combo box, show how that same value looks in that base
            foreach (var opt in BaseOptions)
            {
                opt.ConvertedValue = Convert.ToString(currentValueDecimal, opt.BaseNumber).ToUpper();
            }
        }

        private void AppendNumber(string digit)
        {
            // UI blocks invalid digits for the current base, so we can just append
            if (_isNewEntry || DisplayText == "0")
                DisplayText = digit;
            else
                DisplayText += digit;
            _isNewEntry = false;
        }

        private void SetOperator(string op)
        {
            // If user typed something, do a partial calc first
            if (!_isNewEntry)
                CalculateResult();

            // Store the current display as _currentValue
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
            // Convert the result to a string in the *currently selected* base
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
                // Convert DisplayText from the current base to decimal
                return Convert.ToInt32(DisplayText, _selectedBase);
            }
            catch
            {
                // If parsing fails (e.g. blank string), just treat as 0
                return 0;
            }
        }

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
