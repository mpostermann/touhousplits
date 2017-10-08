using System.Collections.Generic;
using System.Windows.Forms;

namespace TouhouSplits.UI.Hotkey
{
    public interface IGlobalKeyboardHook
    {
        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;
        List<Keys> HookedKeys { get; }

        void hook();
        void unhook();
    }
}