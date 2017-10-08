using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace TouhouSplits.UI.Hotkey
{
    public class GlobalHotkeyManager : IGlobalHotkeyManager
    {
        private GlobalKeyboardHook _keyboardHook;
        private IDictionary<Keys, ICommand> _commandMap;

        public GlobalHotkeyManager()
        {
            _commandMap = new Dictionary<Keys, ICommand>();
            _keyboardHook = new GlobalKeyboardHook();
            KeyDownEventHandler = new System.Windows.Forms.KeyEventHandler(KeyDownEvent);
        }

        public System.Windows.Forms.KeyEventHandler KeyDownEventHandler;
        public void KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_commandMap.ContainsKey(e.KeyCode)) {
                ICommand command = _commandMap[e.KeyCode];
                if (command.CanExecute(null)) {
                    command.Execute(null);
                }
            }
            e.Handled = true;
        }

        public void RegisterHotkey(Keys keys, ICommand command)
        {
            _commandMap.Add(keys, command);
            _keyboardHook.HookedKeys.Add(keys);
            _keyboardHook.KeyDown -= KeyDownEventHandler;
            _keyboardHook.KeyDown += KeyDownEventHandler;
        }

        public void UnregisterHotkey(Keys keys)
        {
            _commandMap.Remove(keys);
            _keyboardHook.HookedKeys.Remove(keys);
        }
    }
}
