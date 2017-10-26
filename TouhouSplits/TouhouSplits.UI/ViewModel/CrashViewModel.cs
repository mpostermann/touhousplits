using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TouhouSplits.Service;

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
            try {
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = FilePaths.EXT_CRASH_FILE;
                dialog.Filter = string.Format("Crash Files ({0})|*{0}|All Files (*.*)|*.*", FilePaths.EXT_CRASH_FILE);

                if (dialog.ShowDialog() == true) {
                    using (var filestream = File.Open(dialog.FileName, FileMode.Create, FileAccess.Write)) {
                        using (StreamWriter writer = new StreamWriter(filestream)) {
                            WriteStackTrace(writer, _exception);
                            writer.Flush();
                        }
                    }
                    ShowMessageDialog("Bug report saved successfully.");
                }
            }
            catch (Exception e) {
                string message = "Unable to save bug report. Please select a different location to save to and try again.\n\nError: " + e.Message;
                ShowErrorDialog(message);
            }
        }

        private static void WriteStackTrace(StreamWriter writer, Exception e)
        {
            if (e != null) {
                writer.WriteLine(e.Message);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine();

                WriteStackTrace(writer, e.InnerException);
            }
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
