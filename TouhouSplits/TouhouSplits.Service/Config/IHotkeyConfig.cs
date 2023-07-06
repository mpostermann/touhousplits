using System.Collections.Generic;
using System.Windows.Forms;
using TouhouSplits.Service.Enums;

namespace TouhouSplits.Service.Config
{
    public interface IHotkeyConfig
    {
        bool HasMethod(Keys keys);
        IList<Keys> GetHotkeys(HotkeyableMethodEnum method);
        HotkeyableMethodEnum GetMethod(Keys keys);
    }
}
