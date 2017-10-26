using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace TouhouSplits.UI.ViewModel
{
    public class CrashViewModel
    {
        public string Message { get; private set; }

        public ICommand SaveBugReportCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public CrashViewModel(string message)
        {
            Message = message;

            SaveBugReportCommand = new RelayCommand(() => SaveBugReport());
            ExitCommand = new RelayCommand(() => ExitApplication());
        }

        private void SaveBugReport()
        {
            // do nothing
        }

        private void ExitApplication()
        {
            //do nothing
        }
    }
}
