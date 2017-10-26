using System;
using System.Globalization;
using System.Windows.Data;
using TouhouSplits.Service;

namespace TouhouSplits.UI.Converter
{
    public class ScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try {
                long score = long.Parse(value.ToString());
                if (score < 0) {
                    return string.Empty;
                }
                return value;
            }
            catch (Exception) {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try {
                return long.Parse(value.ToString());
            }
            catch (Exception) {
                return Constants.UNSET_SCORE;
            }
        }
    }
}
