using System;
using System.Collections.Generic;
using System.IO;
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
        private IFileSerializer<Splits> _splitsSerializer;

        public string GameName { get { return _config.GameName; } } 
        public IHookStrategy Hook { get; private set; } 
        public IList<ISplitsFile> RecentSplits { get; private set; }

        public GameManager(
            IGameConfig config,
            IHookStrategyFactory hookFactory,
            IFileSerializer<List<string>> recentSplitsSerializer,
            IFileSerializer<Splits> splitsSerializer)
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
            IFileSerializer<Splits> splitsSerializer)
        {
            List<ISplitsFile> recentSplits = new List<ISplitsFile>();
            try {
                List<string> recentSplitsPaths = recentSplitsSerializer.Deserialize(recentSplitsFile);
                foreach (string path in recentSplitsPaths) {
                    var splitsFile = new SplitsFile(new FileInfo(path), splitsSerializer);
                    recentSplits.Add(splitsFile);
                }
            }
            catch (DirectoryNotFoundException) {
                /* If the recent splits file doesn't exist, then create a new, empty file */
                recentSplitsSerializer.Serialize(new List<string>(), recentSplitsFile);
            }
            catch (FileNotFoundException) {
                /* If the recent splits file doesn't exist, then create a new, empty file */
                recentSplitsSerializer.Serialize(new List<string>(), recentSplitsFile);
            }
            return recentSplits;
        }

        public ISplitsFile SerializeSplits(ISplits splits, FileInfo filePath)
        {
            var concreteSplits = splits as Splits;
            if (concreteSplits == null) {
                throw new NotSupportedException(string.Format("{0} is not supported for serialization", splits.GetType()));
            }

            _splitsSerializer.Serialize(concreteSplits, filePath);
            return AddToRecentSplits(splits, filePath);
        }

        public ISplitsFile DeserializeSplits(FileInfo filePath)
        {
            var splits = _splitsSerializer.Deserialize(filePath);
            return AddToRecentSplits(splits, filePath);
        }

        /// <summary>
        /// Adds a splits to the recent splits list, if it's not already in it.
        /// </summary>
        private ISplitsFile AddToRecentSplits(ISplits splits, FileInfo filePath)
        {
            var splitsFile = new SplitsFile(filePath, splits);

            /* Check if the in-memory list contains this file already - if it does then update the in-memory list and return it */
            for (int i = 0; i < RecentSplits.Count; i++) {
                if (RecentSplits[i].FileInfo.FullName.Trim().ToLower() == splitsFile.FileInfo.FullName.Trim().ToLower()) {
                    RecentSplits[i] = splitsFile;
                    return splitsFile;
                }
            }

            /* Update recent splits file */
            List<string> recentSplitsPaths = _recentSplitsSerializer.Deserialize(_config.RecentSplitsList);
            recentSplitsPaths.Add(splitsFile.FileInfo.FullName);
            _recentSplitsSerializer.Serialize(recentSplitsPaths, _config.RecentSplitsList);

            /* Update the recent splits in-memory list */
            RecentSplits.Add(splitsFile);

            return splitsFile;
        }
        
    }
}
