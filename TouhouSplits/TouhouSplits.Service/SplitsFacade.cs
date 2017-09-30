using System;
using System.Collections.Generic;
using System.IO;
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
        private IList<IGameConfig> _gameConfigs;
        private IFileSerializer<Splits> _splitsSerializer;
        private IDictionary<string, IGameManager> _gameManagerCache;

        public SplitsFacade(IConfigManager configManager, IFileSerializer<Splits> splitsSerializer) {
            _gameConfigs = configManager.AvailableGames;
            _splitsSerializer = splitsSerializer;
            _gameManagerCache = new Dictionary<string, IGameManager>();
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
            if (!_gameManagerCache.ContainsKey(gameName)) {
                var gameManager = ConstructGameManagerFromConfig(gameName, _gameConfigs);
                _gameManagerCache.Add(gameName, gameManager);
            }
            return _gameManagerCache[gameName];
        }

        private IGameManager ConstructGameManagerFromConfig(string gameName, IList<IGameConfig> gameConfigs)
        {
            foreach (IGameConfig config in gameConfigs) {
                if (config.GameName == gameName.Trim()) {
                    return new GameManager(config,
                        HookStrategyFactory.GetInstance(),
                        new JsonSerializer<List<string>>(),
                        _splitsSerializer
                    );
                }
            }
            throw new NotSupportedException(string.Format("The game \"{0}\" is not supported.", gameName));
        }

        public IFileHandler<ISplits> LoadSplitsFile(FileInfo filePath)
        {
            return new FileHandler<ISplits, Splits>(filePath, _splitsSerializer);
        }

        public IFileHandler<ISplits> NewSplitsFile()
        {
            return new FileHandler<ISplits, Splits>(new Splits(), _splitsSerializer);
        }
    }
}
