using System.Windows;

namespace TouhouSplits.UI.ViewModel
{
    public class ViewModelBase
    {
        public static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }

        public static void ShowMessageDialog(string message, string title = "Message")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        public static MessageBoxResult ShowYesNoDialog(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo);
        }
    }
}
