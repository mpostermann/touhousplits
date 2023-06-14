using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Enums;

namespace TouhouSplits.Service.Config
{
    public class HotkeyConfig : IHotkeyConfig
    {
        private readonly Dictionary<HotkeyableMethodEnum, IList<Keys>> _keyMap;

        public HotkeyConfig(XElement configElement)
        {
            if (configElement.Name != "Hotkeys") {
                throw new ConfigurationErrorsException(string.Format("Unexpected config element {0}", configElement.Name));
            }

            _keyMap = new Dictionary<HotkeyableMethodEnum, IList<Keys>>();
            foreach (XElement hotkeyElement in configElement.Elements("Hotkey")) {
                AddHotkey(_keyMap, hotkeyElement);
            }
        }

        public HotkeyConfig(IList<IHotkey> hotkeys)
        {
            _keyMap = new Dictionary<HotkeyableMethodEnum, IList<Keys>>();
            foreach (IHotkey hotkey in hotkeys) {
                _keyMap.Add(hotkey.Method, new List<Keys>(hotkey.Keys));
                _keyMap[hotkey.Method].Remove(Keys.None);
            }
        }

        private static void AddHotkey(Dictionary<HotkeyableMethodEnum, IList<Keys>> keyMap, XElement hotkeyElement)
        {
            if (hotkeyElement.Attribute("method") == null || string.IsNullOrEmpty(hotkeyElement.Attribute("method").Value)) {
                throw new ConfigurationErrorsException("Hotkey must have a non-empty \"method\" attribute.");
            }
            HotkeyableMethodEnum method;
            if (!Enum.TryParse(hotkeyElement.Attribute("method").Value, out method)) {
                throw new ConfigurationErrorsException($"Method {hotkeyElement.Attribute("method").Value} is not a recognized method");
            }

            if (hotkeyElement.Attribute("keys") == null || string.IsNullOrEmpty(hotkeyElement.Attribute("keys").Value)) {
                throw new ConfigurationErrorsException("Hotkey must have a non-empty \"keys\" attribute.");
            }
            Keys keys = ParseKeys(hotkeyElement.Attribute("keys").Value);

            if (!keyMap.ContainsKey(method)) {
                keyMap.Add(method, new List<Keys>());
            }
            keyMap[method].Add(keys);
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

        public IList<Keys> GetHotkeys(HotkeyableMethodEnum method)
        {
            if (_keyMap.ContainsKey(method)) {
                return new List<Keys>(_keyMap[method]);
            }
            return new List<Keys>();
        }

        public HotkeyableMethodEnum GetMethod(Keys keys)
        {
            var match = _keyMap.FirstOrDefault(entry => entry.Value.Contains(keys));
            if (match.Equals(default(KeyValuePair<HotkeyableMethodEnum, IList<Keys>>))) {
                throw new ConfigurationErrorsException($"{keys} is not mapped to a method");
            }

            return match.Key;
        }

        public bool HasMethod(Keys keys)
        {
            if (keys == Keys.None) {
                return false;
            }

            return _keyMap.Values.Any(n => n.Contains(keys));
        }

        public XElement ToXml()
        {
            var xml = new XElement("Hotkeys");
            foreach (KeyValuePair<HotkeyableMethodEnum, IList<Keys>> entry in _keyMap)
            {
                foreach (Keys keys in entry.Value) {
                    if (keys != Keys.None) {
                        var row = new XElement("Hotkey");
                        row.SetAttributeValue("method", entry.Key);
                        row.SetAttributeValue("keys", keys);
                        xml.Add(row);
                    }
                }
            }

            return xml;
        }
    }
}
