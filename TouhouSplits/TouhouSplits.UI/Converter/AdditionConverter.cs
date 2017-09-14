using System;
using System.Globalization;
using System.Windows.Data;

namespace TouhouSplits.UI.Converter
{
    public class AdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try {
                double modifiee = double.Parse(value.ToString());
                double modifier = double.Parse(parameter.ToString());
                return modifiee + modifier;
            }
            catch (Exception) {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try {
                double modifiee = double.Parse(value.ToString());
                double modifier = double.Parse(parameter.ToString());
                return modifiee + modifier;
            }
            catch (Exception) {
                return value;
            }
        }
    }
}
