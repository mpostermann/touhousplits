using Microsoft.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace TouhouSplits.UI.Control.DataGrid
{
    public class DataGridLongColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            edit.PreviewTextInput += OnPreviewTextInput;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            long value;
            if (!long.TryParse(e.Text, out value)) {
                e.Handled = true;
            }
            else if (value < 0) {
                e.Handled = true;
            }
        }
    }
}
