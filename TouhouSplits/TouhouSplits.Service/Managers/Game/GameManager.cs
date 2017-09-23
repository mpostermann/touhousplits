using System;
using System.Collections.Generic;
using TouhouSplits.MVVM;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service.Managers.Game
{
    public class GameManager : ModelBase, IGameManager
    {
        private IGameConfig _config;

        public string GameName { get { return _config.GameName; } } 
        public IHookStrategy Hook { get; private set; } 

        public GameManager(IGameConfig config, IHookStrategyFactory hookFactory, IFileSerializer<List<SplitsFile>> recentSplitsSerializer)
        {
            _config = config;
            Hook = hookFactory.Create(config.HookConfig);
        }

        public IList<ISplitsFile> RecentSplits => throw new NotImplementedException();

        public ISplitsFile SerializeSplits(ISplits splits, string filePath)
        {
            throw new NotImplementedException();
        }

        public ISplitsFile DeserializeSplits(string filePath)
        {
            throw new NotImplementedException();
        }
        
    }
}
