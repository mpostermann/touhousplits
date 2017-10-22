using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class HotkeyConfig : IHotkeyConfig
    {
        private Dictionary<string, IList<Keys>> _keyMap;

        public HotkeyConfig(XElement configElement)
        {
            if (configElement.Name != "Hotkeys") {
                throw new ConfigurationErrorsException(string.Format("Unexpected config element {0}", configElement.Name));
            }

            _keyMap = new Dictionary<string, IList<Keys>>();
            foreach (XElement hotkeyElement in configElement.Elements("Hotkey")) {
                AddHotkey(_keyMap, hotkeyElement);
            }
        }

        private static void AddHotkey(Dictionary<string, IList<Keys>> keyMap, XElement hotkeyElement)
        {
            if (hotkeyElement.Attribute("method") == null || string.IsNullOrEmpty(hotkeyElement.Attribute("method").Value)) {
                throw new ConfigurationErrorsException("Hotkey must have a non-empty \"method\" attribute.");
            }
            string methodName = hotkeyElement.Attribute("method").Value;

            if (hotkeyElement.Attribute("keys") == null || string.IsNullOrEmpty(hotkeyElement.Attribute("keys").Value)) {
                throw new ConfigurationErrorsException("Hotkey must have a non-empty \"keys\" attribute.");
            }
            Keys keys = ParseKeys(hotkeyElement.Attribute("keys").Value);

            if (!keyMap.ContainsKey(methodName)) {
                keyMap.Add(methodName, new List<Keys>());
            }
            keyMap[methodName].Add(keys);
        }

        private static Keys ParseKeys(string value)
        {
            string[] allKeys = value.Split('+');
            Keys keys = ParseSingleKey(allKeys[0]);
            for (int i = 1; i < allKeys.Length; i++) {
                Keys nextKey = ParseSingleKey(allKeys[i]);
                keys = keys | nextKey;
            }

            return keys;
        }

        private static Keys ParseSingleKey(string value)
        {
            Keys keys;
            if (!Enum.TryParse<Keys>(value, out keys)) {
                throw new ConfigurationErrorsException(string.Format("Unrecognized key \"{0}\"", value));
            }
            return keys;
        }

        public IList<Keys> GetHotkeys(string method)
        {
            return _keyMap[method];
        }

        public bool HasHotkey(string method)
        {
            return _keyMap.ContainsKey(method);
        }
    }
}
