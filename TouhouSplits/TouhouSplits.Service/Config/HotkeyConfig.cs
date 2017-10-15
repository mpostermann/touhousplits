using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class HotkeyConfig : IHotkeyConfig
    {
        private Dictionary<string, Keys> _keyMap;

        public HotkeyConfig(XElement configElement)
        {
            throw new NotImplementedException();
        }

        public Keys GetHotkey(string method)
        {
            return _keyMap[method];
        }

        public bool HasHotkey(string method)
        {
            return _keyMap.ContainsKey(method);
        }
    }
}
