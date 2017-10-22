using System;
using System.Globalization;
using System.Windows.Data;

namespace TouhouSplits.UI.Converter
{
    public class IsGreaterThanOrEqualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) {
                return false;
            }

            try {
                long value0 = long.Parse(values[0].ToString());
                long value1 = long.Parse(values[1].ToString());
                return value0 >= value1;
            }
            catch (Exception) {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
