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
        private IFileSerializer<List<string>> _favoriteSplitsSerializer;

        public string GameName { get { return _config.GameName; } } 
        public IHookStrategy Hook { get; private set; } 
        public IList<IFileHandler<ISplits>> FavoriteSplits { get; private set; }

        public GameManager(
            IGameConfig config,
            IHookStrategyFactory hookFactory,
            IFileSerializer<List<string>> favoriteSplitsSerializer,
            IFileSerializer<Splits> splitsSerializer)
        {
            _config = config;
            _favoriteSplitsSerializer = favoriteSplitsSerializer;
            Hook = hookFactory.Create(config.HookConfig);
            FavoriteSplits = LoadFavoriteSplits(_config.FavoriteSplitsList, _favoriteSplitsSerializer, splitsSerializer);
        }

        private static List<IFileHandler<ISplits>> LoadFavoriteSplits(
            FileInfo favoriteSplitsFile,
            IFileSerializer<List<string>> favoriteSplitsSerializer,
            IFileSerializer<Splits> splitsSerializer)
        {
            List<IFileHandler<ISplits>> favoriteSplits = new List<IFileHandler<ISplits>>();
            try {
                List<string> favoriteSplitsPaths = favoriteSplitsSerializer.Deserialize(favoriteSplitsFile);
                foreach (string path in favoriteSplitsPaths) {
                    var splitsFile = new FileHandler<ISplits, Splits>(new FileInfo(path), splitsSerializer);
                    favoriteSplits.Add(splitsFile);
                }
            }
            catch (DirectoryNotFoundException) {
                /* If the favorite splits file doesn't exist, then create a new, empty file */
                favoriteSplitsSerializer.Serialize(new List<string>(), favoriteSplitsFile);
            }
            catch (FileNotFoundException) {
                /* If the favorite splits file doesn't exist, then create a new, empty file */
                favoriteSplitsSerializer.Serialize(new List<string>(), favoriteSplitsFile);
            }
            return favoriteSplits;
        }

        /// <summary>
        /// Adds a splits to the favorite splits list, if it's not already in it.
        /// </summary>
        public void AddOrUpdateFavorites(IFileHandler<ISplits> splitsFile)
        {
            if (splitsFile.Object.GameName != this.GameName) {
                throw new InvalidOperationException("Files can only be added to Favorites for a GameManager with a matching name.");
            }

            /* Check if the in-memory list contains this file already */
            int index = GetFavoriteSplitsFileIndex(splitsFile.FileInfo);
            if (index != -1) {
                FavoriteSplits[index] = splitsFile;
            }
            else {
                AddToFavoritesSplitsFile(splitsFile.FileInfo);
                FavoriteSplits.Add(splitsFile);
            }
        }

        private int GetFavoriteSplitsFileIndex(FileInfo filePath)
        {
            for (int i = 0; i < FavoriteSplits.Count; i++) {
                if (FavoriteSplits[i].FileInfo.FullName.Trim().ToLower() == filePath.FullName.Trim().ToLower()) {
                    return i;
                }
            }
            return -1;
        }

        private void AddToFavoritesSplitsFile(FileInfo filePath)
        {
            List<string> favoriteSplitsPaths = _favoriteSplitsSerializer.Deserialize(_config.FavoriteSplitsList);
            favoriteSplitsPaths.Add(filePath.FullName);
            _favoriteSplitsSerializer.Serialize(favoriteSplitsPaths, _config.FavoriteSplitsList);
        }

    }
}
