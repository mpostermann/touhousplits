using System.Windows.Forms;

namespace TouhouSplits.Service.Config
{
    public interface IHotkeyConfig
    {
        bool HasHotkey(string method);
        Keys GetHotkey(string method);
    }
}
