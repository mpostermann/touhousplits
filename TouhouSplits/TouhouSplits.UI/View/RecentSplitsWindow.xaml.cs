using System;
using System.Windows;
using TouhouSplits.UI.ViewModel;

namespace TouhouSplits.UI.View
{
    /// <summary>
    /// Interaction logic for RecentSplitsWindow.xaml
    /// </summary>
    public partial class RecentSplitsWindow : Window
    {
        private bool _isClosed = false;

        public RecentSplitsWindow()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Closed += OnDialogWindowClosed;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /* Set an event handler for RequestCloseDialog */
            if (e.NewValue is IDialogResultViewModel) {
                var d = e.NewValue as IDialogResultViewModel;

                d.RequestCloseDialog += new EventHandler<RequestCloseDialogEventArgs>(DialogResultTrueEvent);
            }
        }

        private void DialogResultTrueEvent(object sender, RequestCloseDialogEventArgs eventargs)
        {
            if (_isClosed) {
                DialogResult = false;
            }
            else {
                DialogResult = eventargs.DialogResult;
            }
        }

        private void OnDialogWindowClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }
    }
}
