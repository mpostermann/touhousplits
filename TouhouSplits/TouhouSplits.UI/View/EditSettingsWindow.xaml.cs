using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.ViewModel;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace TouhouSplits.UI.View
{
    /// <summary>
    /// Interaction logic for EditSettingsWindow.xaml
    /// </summary>
    public partial class EditSettingsWindow : Window
    {
        private bool _isClosed = false;

        public EditSettingsWindow()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Closed += OnDialogWindowClosed;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /* Set an event handler for RequestCloseDialog */
            if (e.NewValue is IDialogResultViewModel dataContext) {
                dataContext.RequestCloseDialog += DialogResultTrueEvent;
            }
        }

        private void DialogResultTrueEvent(object sender, RequestCloseDialogEventArgs eventArgs)
        {
            if (_isClosed) {
                DialogResult = false;
            }
            else {
                DialogResult = eventArgs.DialogResult;
            }
        }

        private void OnDialogWindowClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }

        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Don't allow updating text manually
            e.Handled = true;
        }

        private void textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftAlt || e.Key == Key.RightAlt) {
                e.Handled = true;
                return;
            }

            // Get the key combination being pressed
            var newKeys = (Keys) KeyInterop.VirtualKeyFromKey(e.Key);
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                newKeys |= Keys.Control;
            }
            else if (Keyboard.IsKeyDown(Key.RightCtrl)) {
                newKeys |= Keys.Control;
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt)) {
                newKeys |= Keys.Alt;
            }
            else if (Keyboard.IsKeyDown(Key.RightAlt)) {
                newKeys |= Keys.Alt;
            }

            // Get the hotkey to update
            var originalKeys = (Keys) ((TextBox) sender).GetBindingExpression(TextBox.TextProperty).DataItem;

            // Invoke to update
            var param = new EditHotkeyCommandParameters(originalKeys, newKeys);
            ((EditSettingsViewModel) DataContext).EditHotkeyCommand.Execute(param);
        }

        private void textbox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Don't allow cutting, copying, or pasting text
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut  || 
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
