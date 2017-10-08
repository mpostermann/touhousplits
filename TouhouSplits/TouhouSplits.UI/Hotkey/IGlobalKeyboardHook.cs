using System.Windows.Forms;

namespace TouhouSplits.UI.Hotkey
{
    public interface IGlobalKeyboardHook
    {
        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;

        void hook();
        void unhook();
    }
}