using System;
using System.Globalization;
using System.Windows.Data;
using TouhouSplits.Service;

namespace TouhouSplits.UI.Converter
{
    /// <summary>
    /// Displays the difference between the segment's running score and the PB score.
    /// A value will only be returned if:
    /// a) The RunningScore is within some percentage of the PersonalBestScore, OR
    /// b) The segment has been completed
    /// 
    /// Expected values order:
    /// 1) RunningScore
    /// 2) PersonalBestScore
    /// 3) IsCompleted
    /// </summary>
    public class ScoreDifferenceConverter : IMultiValueConverter
    {
        private const double DISPLAY_PERCENTAGE_THESHOLD = 0.90;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length != 3) {
                return string.Empty;
            }

            long score1 = ParseScore(values[0]);
            long score2 = ParseScore(values[1]);
            bool forceDisplayScore = ParseBoolean(values[2]);
            if (score1 == Constants.UNSET_SCORE || score2 == Constants.UNSET_SCORE) {
                return string.Empty;
            }

            if (score1 < score2 * DISPLAY_PERCENTAGE_THESHOLD && !forceDisplayScore) {
                return string.Empty;
            }

            return score1 - score2;
        }

        private long ParseScore(object value) {
            try {
                return long.Parse(value.ToString());
            }
            catch (Exception) {
                return Constants.UNSET_SCORE;
            }
        }

        private bool ParseBoolean(object value) {
            try {
                return bool.Parse(value.ToString());
            }
            catch (Exception) {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new InvalidOperationException();
        }
    }
}
