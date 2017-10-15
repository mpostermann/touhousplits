using System.Windows;

namespace TouhouSplits.UI.ViewModel
{
    public class ViewModelBase
    {
        public static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }
    }
}
