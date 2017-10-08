using System.Windows.Forms;
using System.Windows.Input;

namespace TouhouSplits.UI.Hotkey
{
    public interface IGlobalHotkeyManager
    {
        void RegisterHotkey(Keys keys, ICommand command);
        void UnregisterHotkey(Keys keys);
    }
}
