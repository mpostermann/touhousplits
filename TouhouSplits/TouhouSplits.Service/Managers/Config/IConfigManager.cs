using System.Collections.Generic;
using TouhouSplits.Service.Config;

namespace TouhouSplits.Manager.Config
{
    public class IConfigManager
    {
        IList<IGameConfig> AvailableGames { get; }
    }
}
