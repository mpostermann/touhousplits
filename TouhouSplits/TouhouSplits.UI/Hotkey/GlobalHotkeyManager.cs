using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace TouhouSplits.UI.Hotkey
{
    public class GlobalHotkeyManager : IGlobalHotkeyManager
    {
        private IGlobalKeyboardHook _keyboardHook;
        private IDictionary<Keys, ICommand> _commandMap;

        public GlobalHotkeyManager(IGlobalKeyboardHook globalKeyboardHook)
        {
            _commandMap = new Dictionary<Keys, ICommand>();
            _keyboardHook = globalKeyboardHook;
            KeyDownEventHandler = new System.Windows.Forms.KeyEventHandler(KeyDownEvent);
            _keyboardHook.KeyDown -= KeyDownEventHandler;
            _keyboardHook.KeyDown += KeyDownEventHandler;
        }

        private System.Windows.Forms.KeyEventHandler KeyDownEventHandler;

        public List<Keys> RegisteredHotkeys => throw new NotImplementedException();

        private void KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_commandMap.ContainsKey(e.KeyCode)) {
                ICommand command = _commandMap[e.KeyCode];
                if (command.CanExecute(null)) {
                    command.Execute(null);
                    e.Handled = true;
                }
            }
        }

        public void RegisterHotkey(Keys keys, ICommand command)
        {
            if (_commandMap.ContainsKey(keys)) {
                throw new ArgumentException(string.Format("Hotkey \"{0}\" is already registered.", keys.ToString()));
            }
            _commandMap.Add(keys, command);
            _keyboardHook.HookedKeys.Add(keys);
        }

        public void UnregisterHotkey(Keys keys)
        {
            _commandMap.Remove(keys);
            _keyboardHook.HookedKeys.Remove(keys);
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void RegisterEnableToggleHotkey(Keys keys)
        {
            throw new NotImplementedException();
        }

        public void UnregisterEnableToggleHotkey()
        {
            throw new NotImplementedException();
        }
    }
}
