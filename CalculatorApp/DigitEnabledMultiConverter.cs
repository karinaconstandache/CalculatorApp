// DigitEnabledMultiConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace CalculatorApp.Converters
{
    public class DigitEnabledMultiConverter : IMultiValueConverter
    {
        // values[0]: SelectedBase (int)
        // values[1]: Button Content (string)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is int selectedBase))
                return false;

            string digitStr = values[1]?.ToString()?.ToUpper();
            if (string.IsNullOrEmpty(digitStr))
                return false;

            int digitValue;
            if (int.TryParse(digitStr, out digitValue))
            {
                return digitValue < selectedBase;
            }
            else if ("ABCDEF".Contains(digitStr))
            {
                // Map A->10, B->11, etc.
                digitValue = digitStr[0] - 'A' + 10;
                return digitValue < selectedBase;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

