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
    public class GameManager : IGameManager
    {
        private IGameConfig _config;
        private IFileSerializer<List<string>> _recentSplitsSerializer;
        private IFileSerializer<ISplits> _splitsSerializer;

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
            _splitsSerializer = splitsSerializer;
            Hook = hookFactory.Create(config.HookConfig);
            RecentSplits = LoadRecentSplits(_config.RecentSplitsList, _recentSplitsSerializer, _splitsSerializer);
        }

        private static List<ISplitsFile> LoadRecentSplits(
            FileInfo recentSplitsFile,
            IFileSerializer<List<string>> recentSplitsSerializer,
            IFileSerializer<ISplits> splitsSerializer)
        {
            List<ISplitsFile> recentSplits = new List<ISplitsFile>();
            List<string> recentSplitsPaths = recentSplitsSerializer.Deserialize(recentSplitsFile.FullName);
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
            _splitsSerializer.Serialize(splits, filePath);
            return AddToRecentSplits(splits, filePath);
        }

        public ISplitsFile DeserializeSplits(string filePath)
        {
            var splits = _splitsSerializer.Deserialize(filePath);
            return AddToRecentSplits(splits, filePath);
        }

        private ISplitsFile AddToRecentSplits(ISplits splits, string filePath)
        {
            /* Update recent splits file */
            List<string> recentSplitsPaths = _recentSplitsSerializer.Deserialize(_config.RecentSplitsList.FullName);
            recentSplitsPaths.Add(filePath);
            _recentSplitsSerializer.Serialize(recentSplitsPaths, filePath);

            /* Update the recent splits in-memory list */
            var splitsFile = new SplitsFile(filePath, _splitsSerializer);
            RecentSplits.Add(new SplitsFile(filePath, _splitsSerializer));

            return splitsFile;
        }
        
    }
}
