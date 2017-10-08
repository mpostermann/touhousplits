
namespace TouhouSplits.UI.Hotkey
{
    public static class GlobalHotkeyManagerFactory
    {
        private static IGlobalHotkeyManager _defaultManager = new GlobalHotkeyManager(new GlobalKeyboardHook());

        private static IGlobalHotkeyManager _instance;
        public static IGlobalHotkeyManager Instance {
            get {
                if (_instance == null) {
                    _instance = _defaultManager;
                }
                return _instance;
            }
        }

        public static void SetInstance(IGlobalHotkeyManager manager)
        {
            _instance = manager;
        }

        public static void ResetToDefaultInstance()
        {
            _instance = _defaultManager;
        }
    }
}
