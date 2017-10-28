using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using TouhouSplits.UI.Dialog;

namespace TouhouSplits.UI.ViewModel
{
    public class CrashViewModel : ViewModelBase
    {
        private Exception _exception;

        public string Message { get; private set; }

        public ICommand SaveBugReportCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public CrashViewModel(string message, Exception exception)
        {
            Message = message;
            _exception = exception;

            SaveBugReportCommand = new RelayCommand(() => SaveBugReport());
            ExitCommand = new RelayCommand(() => ExitApplication());
        }

        private void SaveBugReport()
        {
            var bugReporter = new BugReporter();
            string saveError;
            if (bugReporter.ShowSaveBugReportDialog(_exception, out saveError)) {
                ShowMessageDialog("Bug report saved successfully.");
            }
            else {
                ShowErrorDialog("Unable to save bug report to the specified location. Please select a different file location and try again.\n\n Error: " + saveError);
            }
        }

        

        private void ExitApplication()
        {
            //Do nothing
        }
    }
}
