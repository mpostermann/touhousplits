using System;

namespace TouhouSplits.UI.ViewModel
{
    public interface IDialogResultViewModel
    {
        event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
    }

    public class RequestCloseDialogEventArgs : EventArgs
    {
        public bool DialogResult { get; set; }

        public RequestCloseDialogEventArgs(bool dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}
