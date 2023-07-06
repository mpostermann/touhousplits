using System.Collections.Generic;
using System.Windows.Forms;
using TouhouSplits.Service.Enums;

namespace TouhouSplits.Service.Data
{
    public interface IHotkey
    {
        HotkeyableMethodEnum Method { get; }
        string MethodName { get; }
        IList<Keys> Keys { get; }
    }
}
