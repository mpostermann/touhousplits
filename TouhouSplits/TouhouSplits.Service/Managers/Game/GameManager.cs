using System;
using System.Collections.Generic;
using System.IO;
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
        private IFileSerializer<List<string>> _recentSplitsSerializer;

        public string GameName { get { return _config.GameName; } } 
        public IHookStrategy Hook { get; private set; } 
        public IList<ISplitsFile> RecentSplits { get; private set; }

        public GameManager(
            IGameConfig config,
            IHookStrategyFactory hookFactory,
            IFileSerializer<List<string>> recentSplitsSerializer,
            IFileSerializer<ISplits> splitsSerializer)
        {
            _config = config;
            _recentSplitsSerializer = recentSplitsSerializer;
            Hook = hookFactory.Create(config.HookConfig);
            RecentSplits = LoadRecentSplits(_config.RecentSplitsList, _recentSplitsSerializer, splitsSerializer);
        }

        private static List<ISplitsFile> LoadRecentSplits(
            FileInfo recentSplitsFile,
            IFileSerializer<List<string>> serializer,
            IFileSerializer<ISplits> splitsSerializer)
        {
            List<ISplitsFile> recentSplits = new List<ISplitsFile>();
            List<string> recentSplitsPaths = serializer.Deserialize(recentSplitsFile.FullName);
            foreach (string path in recentSplitsPaths) {
                recentSplits.Add(new SplitsFile(
                    path,
                    splitsSerializer
                ));
            }
            return recentSplits;
        }

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
