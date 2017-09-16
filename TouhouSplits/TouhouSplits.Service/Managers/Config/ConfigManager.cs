using System;
using System.Collections.Generic;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Config;

namespace TouhouSplits.Service.Managers.Config
{
    public class ConfigManager : IConfigManager
    {
        public IList<IGameConfig> AvailableGames { get; private set; }

        public ConfigManager()
        {
            /* Todo: load config XDocument and list of AvailableGames */
            throw new NotImplementedException();
        }

    }
}
