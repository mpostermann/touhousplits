using System.Collections.Generic;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Manager.Config
{
    public interface IConfigManager
    {
        IList<IGameConfig> AvailableGames { get; }
        IHotkeyConfig Hotkeys { get; }

        void UpdateAndPersistHotkeys(IList<IHotkey> hotkeys);
    }
}
