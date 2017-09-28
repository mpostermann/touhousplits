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
            Hook = hookFactory.Create(config.HookConfig);
            RecentSplits = LoadRecentSplits(_config.RecentSplitsList, _recentSplitsSerializer, splitsSerializer);
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

        /// <summary>
        /// Adds a splits to the recent splits list, if it's not already in it.
        /// </summary>
        public void AddOrUpdateRecentSplits(ISplitsFile splitsFile)
        {
            /* Check if the in-memory list contains this file already */
            int index = GetRecentSplitsFileIndex(splitsFile.FileInfo);
            if (index != -1) {
                RecentSplits[index] = splitsFile;
            }
            else {
                AddToRecentSplitsFile(splitsFile.FileInfo);
                RecentSplits.Add(splitsFile);
            }
        }

        private int GetRecentSplitsFileIndex(FileInfo filePath)
        {
            for (int i = 0; i < RecentSplits.Count; i++) {
                if (RecentSplits[i].FileInfo.FullName.Trim().ToLower() == filePath.FullName.Trim().ToLower()) {
                    return i;
                }
            }
            return -1;
        }

        private void AddToRecentSplitsFile(FileInfo filePath)
        {
            List<string> recentSplitsPaths = _recentSplitsSerializer.Deserialize(_config.RecentSplitsList);
            recentSplitsPaths.Add(filePath.FullName);
            _recentSplitsSerializer.Serialize(recentSplitsPaths, _config.RecentSplitsList);
        }

    }
}
