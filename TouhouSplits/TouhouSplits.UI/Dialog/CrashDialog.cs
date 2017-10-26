using System;
using System.Windows.Threading;
using TouhouSplits.UI.View;
using TouhouSplits.UI.ViewModel;

namespace TouhouSplits.UI.Dialog
{
    public class CrashDialog
    {
        private static DispatcherUnhandledExceptionEventHandler _unhandledExceptionHandler;
        public static DispatcherUnhandledExceptionEventHandler UnhandledEventHandler {
            get {
                if (_unhandledExceptionHandler == null) {
                    _unhandledExceptionHandler = (obj, args) => {
                        var dialog = new CrashDialog();
                        dialog.ShowDialogAndExit(args.Exception);
                    };
                }
                return _unhandledExceptionHandler;
            }
        }

        protected void ShowDialogAndExit(Exception e)
        {
            string message = string.Format("An unexpected error has occured and this application must close.\n\n" +
                "Error message: \n{0}",
                e.Message);

            var dialog = new CrashWindow();
            dialog.DataContext = new CrashViewModel(message);
            dialog.ShowDialog();
        }
    }
}
