using System;
using System.Collections.Generic;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service
{
    public class SplitsFacade : ISplitsFacade
    {
        public const string DEFAULT_FILE_EXT = ".tsf";

        private IList<IGameConfig> _gameConfigs;

        public SplitsFacade(IConfigManager configManager)
        {
            _gameConfigs = configManager.AvailableGames;
        }

        private IList<string> _availableGames;
        public IList<string> AvailableGames
        {
            get {
                if (_availableGames == null) {
                    _availableGames = new string[_gameConfigs.Count];
                    for (int i = 0; i < _gameConfigs.Count; i++) {
                        _availableGames[i] = _gameConfigs[i].GameName;
                    }
                }
                return _availableGames;
            }
        }
        
        public IGameManager LoadGameManager(string gameName)
        {
            foreach (IGameConfig config in _gameConfigs) {
                if (config.GameName == gameName.Trim()) {
                    return new GameManager(config, HookStrategyFactory.GetInstance(), new JsonSerializer<List<SplitsFile>>());
                }
            }
            throw new NotSupportedException(string.Format("The game \"{0}\" is not supported.", gameName));
        }
        
    }
}
