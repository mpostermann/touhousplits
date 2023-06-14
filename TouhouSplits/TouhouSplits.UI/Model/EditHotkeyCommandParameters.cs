using System.Windows.Forms;

namespace TouhouSplits.UI.Model
{
    public class EditHotkeyCommandParameters
    {
        public Keys OriginalKeys { get; }
        public Keys NewKeys { get; }

        public EditHotkeyCommandParameters(Keys originalKeys, Keys newKeys)
        {
            OriginalKeys = originalKeys;
            NewKeys = newKeys;
        }
    }
}
