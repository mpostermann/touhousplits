using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TouhouSplits.Manager.Config;
using TouhouSplits.MVVM;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Enums;

namespace TouhouSplits.UI.Model 
{
    public class HotkeysConfigModel : ModelBase
    {
        private IConfigManager _configManager;

        public IList<IHotkey> Hotkeys { get; }

        public HotkeysConfigModel(IConfigManager configManager)
        {
            _configManager = configManager;

            Hotkeys = new List<IHotkey>();
            foreach (HotkeyableMethodEnum method in Enum.GetValues(typeof(HotkeyableMethodEnum))) {
                var hotkeys = _configManager.Hotkeys.GetHotkeys(method);
                if (hotkeys == null || hotkeys.Count == 0) {
                    hotkeys.Add(Keys.None);
                }
                Hotkeys.Add(new HotkeyModel(method, hotkeys));
            }
        }

        public bool HasHotkey(Keys keys)
        {
            return Hotkeys.Any(n => n.Keys.Contains(keys));
        }

        public IHotkey GetHotkeyOrNull(Keys keys)
        {
            return Hotkeys.FirstOrDefault(n => n.Keys.Contains(keys));
        }

        public void RemoveHotkey(Keys keys)
        {
            if (HasHotkey(keys)) {
                var method = GetHotkeyOrNull(keys);
                method.Keys.Remove(keys);
                if (method.Keys.Count == 0) {
                    method.Keys.Add(Keys.None);
                }
            }
        }

        public void AddEmptyHotkey(Keys keys)
        {
            var hotkey = GetHotkeyOrNull(keys);
            if (hotkey != null && !hotkey.Keys.Contains(Keys.None)) {
                GetHotkeyOrNull(keys).Keys.Add(Keys.None);
            }
        }

        public void EditHotkey(Keys originalKeys, Keys newKeys)
        {
            if (HasHotkey(originalKeys)) {
                if (HasHotkey(newKeys)) {
                    throw new InvalidOperationException($"Cannot set hotkey {newKeys} because it is already mapped to a method.");
                }

                /* Replace originalKeys with newKeys at the same index within the list */
                var method = GetHotkeyOrNull(originalKeys);
                var index = method.Keys.IndexOf(originalKeys);
                method.Keys.Insert(index, newKeys);
                method.Keys.Remove(originalKeys);
            }
        }

        public void PersistChanges()
        {
            _configManager.UpdateAndPersistHotkeys(Hotkeys);
        }
    }
}
