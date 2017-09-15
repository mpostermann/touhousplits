using System.Collections.Generic;
using TouhouSplits.Service.Config;

namespace TouhouSplits.Manager.Config
{
    public interface IConfigManager
    {
        IList<IGameConfig> AvailableGames { get; }
    }
}
