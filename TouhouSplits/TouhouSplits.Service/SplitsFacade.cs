using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private IDictionary<GameId, IGameManager> _gameManagerCache;

        public SplitsFacade(IConfigManager configManager, IFileSerializer<Splits> splitsSerializer) {
            _gameConfigs = configManager.AvailableGames;
            _splitsSerializer = splitsSerializer;
            _gameManagerCache = new Dictionary<GameId, IGameManager>();
        }

        private IList<GameId> _availableGames;
        public IList<GameId> AvailableGames
        {
            get {
                if (_availableGames == null) {
                    _availableGames = new GameId[_gameConfigs.Count];
                    for (int i = 0; i < _gameConfigs.Count; i++) {
                        _availableGames[i] = _gameConfigs[i].Id;
                    }
                }
                return _availableGames;
            }
        }

        private IList<string> _availableGameNames;
        public IList<string> AvailableGameNames {
            get {
                if (_availableGameNames == null) {
                    _availableGameNames = new string[_gameConfigs.Count];
                    for (int i = 0; i < _gameConfigs.Count; i++) {
                        _availableGameNames[i] = _gameConfigs[i].GameName;
                    }
                }
                return _availableGameNames;
            }
        }

        public GameId GetIdFromName(string gameName)
        {
            if (!AvailableGameNames.Contains(gameName)) {
                throw new KeyNotFoundException(string.Format("Game with name {0} is not supported.", gameName.Trim()));
            }
            return _gameConfigs.First(n => n.GameName == gameName.Trim()).Id;
        }

        public IGameManager LoadGameManager(GameId gameId)
        {
            if (!_gameManagerCache.ContainsKey(gameId)) {
                var gameManager = ConstructGameManagerFromConfig(gameId, _gameConfigs);
                _gameManagerCache.Add(gameId, gameManager);
            }
            return _gameManagerCache[gameId];
        }

        private IGameManager ConstructGameManagerFromConfig(GameId gameId, IList<IGameConfig> gameConfigs)
        {
            foreach (IGameConfig config in gameConfigs) {
                if (config.Id == gameId) {
                    return new GameManager(config,
                        HookStrategyFactory.GetInstance(),
                        new JsonSerializer<List<string>>(),
                        _splitsSerializer
                    );
                }
            }
            throw new NotSupportedException(string.Format("The game with Id \"{0}\" is not supported.", gameId));
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
