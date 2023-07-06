using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using TouhouSplits.MVVM;
using TouhouSplits.Service.Enums;

namespace TouhouSplits.Service.Data
{
    public class HotkeyModel : ModelBase, IHotkey
    {
        private static IDictionary<HotkeyableMethodEnum, string> _methodNames =
            new Dictionary<HotkeyableMethodEnum, string>() {
                { HotkeyableMethodEnum.StartOrStopRecordingSplits, "Start/stop recording splits" },
                { HotkeyableMethodEnum.SplitToNextSegment, "Next split" },
                { HotkeyableMethodEnum.ToggleHotkeys, "Enable/disable all hotkeys" }
            };

        public HotkeyableMethodEnum Method { get; }
        public string MethodName => _methodNames[Method];
        public IList<Keys> Keys { get; }

        public HotkeyModel(HotkeyableMethodEnum method, IList<Keys> keys)
        {
            Method = method;
            Keys = new ObservableCollection<Keys>(keys);
            ((ObservableCollection<Keys>) Keys).CollectionChanged += NotifyKeysChanged;
        }

        private void NotifyKeysChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Keys");
        }
    }
}
