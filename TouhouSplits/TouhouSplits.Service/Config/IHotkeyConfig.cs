using System.Collections.Generic;
using System.Windows.Forms;

namespace TouhouSplits.Service.Config
{
    public interface IHotkeyConfig
    {
        bool HasHotkey(string method);
        IList<Keys> GetHotkeys(string method);
    }
}
