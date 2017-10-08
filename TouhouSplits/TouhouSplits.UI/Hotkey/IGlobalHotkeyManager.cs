using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace TouhouSplits.UI.Hotkey
{
    public interface IGlobalHotkeyManager
    {
        List<Keys> RegisteredHotkeys { get; }
        void RegisterHotkey(Keys keys, ICommand command);
        void UnregisterHotkey(Keys keys);
        void Enable();
        void Disable();
        void RegisterEnableToggleHotkey(Keys keys);
        void UnregisterEnableToggleHotkey();
    }
}
