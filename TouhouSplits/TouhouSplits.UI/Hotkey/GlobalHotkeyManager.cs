using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace TouhouSplits.UI.Hotkey
{
    public class GlobalHotkeyManager : IGlobalHotkeyManager
    {
        private IGlobalKeyboardHook _keyboardHook;
        private IDictionary<Keys, ICommand> _commandMap;
        private bool _isEnabled;
        private ICommand _toggleIsEnabledCommand;

        private Keys _enableToggleHotkey;

        public GlobalHotkeyManager(IGlobalKeyboardHook globalKeyboardHook)
        {
            _keyboardHook = globalKeyboardHook;
            _commandMap = new Dictionary<Keys, ICommand>();
            _isEnabled = true;

            _toggleIsEnabledCommand = new RelayCommand(() => ToggleIsEnabled());

            KeyDownEventHandler = new KeyEventHandler(KeyDownEvent);
            _keyboardHook.KeyDown -= KeyDownEventHandler;
            _keyboardHook.KeyDown += KeyDownEventHandler;
        }

        private KeyEventHandler KeyDownEventHandler;

        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            if (_commandMap.ContainsKey(e.KeyCode)) {
                ICommand command = _commandMap[e.KeyCode];

                /* If this is the special EnableToggleHotkey,
                 * then execute irregardless of whether this manager is enabled or not. */
                if (e.KeyCode == _enableToggleHotkey && command.CanExecute(null)) {
                    command.Execute(null);
                    e.Handled = true;
                }
                else if (_isEnabled && command.CanExecute(null)) {
                    command.Execute(null);
                    e.Handled = true;
                }
            }
        }

        public ICollection<Keys> RegisteredHotkeys {
            get { return _commandMap.Keys; }
        }

        public void RegisterHotkey(Keys keys, ICommand command)
        {
            if (_commandMap.ContainsKey(keys)) {
                throw new ArgumentException(string.Format("Hotkey \"{0}\" is already registered.", keys.ToString()));
            }
            _commandMap.Add(keys, command);
        }

        public void UnregisterHotkey(Keys keys)
        {
            _commandMap.Remove(keys);
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void RegisterEnableToggleHotkey(Keys keys)
        {
            if (_enableToggleHotkey == keys) {
                return;
            }

            if (_commandMap.ContainsKey(keys)) {
                throw new ArgumentException(string.Format("Hotkey \"{0}\" is already registered.", keys.ToString()));
            }

            _commandMap.Remove(_enableToggleHotkey);
            _enableToggleHotkey = keys;
            _commandMap.Add(_enableToggleHotkey, _toggleIsEnabledCommand);
        }

        public void UnregisterEnableToggleHotkey()
        {
            _commandMap.Remove(_enableToggleHotkey);
            _enableToggleHotkey = Keys.None;
        }

        private void ToggleIsEnabled()
        {
            _isEnabled = !_isEnabled;
        }
    }
}
