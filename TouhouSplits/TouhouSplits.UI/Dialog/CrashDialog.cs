using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
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
                        args.Handled = true;
                    };
                }
                return _unhandledExceptionHandler;
            }
        }

        protected void ShowDialogAndExit(Exception e)
        {
            string messageFormat = "An unexpected error has occured and this application must close.\n\n" +
                "Error message: \n{0}";

            string message;
            if ((e is TargetInvocationException || e is XamlParseException)
                && e.InnerException != null)
            {
                message = string.Format(messageFormat, e.InnerException.Message);
            }
            else {
                message = string.Format(messageFormat, e.Message);
            }

            var dialog = new CrashWindow();
            dialog.DataContext = new CrashViewModel(message, e);
            dialog.ShowDialog();
            Application.Current.Shutdown();
        }
    }
}
