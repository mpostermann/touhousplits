using Microsoft.Windows.Controls;
using System.Windows;

namespace TouhouSplits.UI.Control.DataGrid
{
    public class DataGridLongColumn : DataGridTextColumn
    {        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            cell.PreviewTextInput -= OnPreviewTextInput;
            cell.PreviewTextInput += OnPreviewTextInput;
            return base.GenerateElement(cell, dataItem);
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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
