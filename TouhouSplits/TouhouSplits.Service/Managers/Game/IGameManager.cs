using System.Collections.Generic;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        GameId Id { get; }
        string GameName { get; }
        IList<IFileHandler<ISplits>> FavoriteSplits { get; }
        bool FavoriteSplitsFileLoaded { get; }
        void AddOrUpdateFavorites(IFileHandler<ISplits> splitsFile);
        bool GameIsHookable { get; }
        bool GameIsRunning();
        long GetCurrentScore();
    }
}
