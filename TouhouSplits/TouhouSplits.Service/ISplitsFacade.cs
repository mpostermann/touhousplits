using System.Collections.Generic;
using System.IO;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.Service
{
    public interface ISplitsFacade
    {
        IList<GameId> AvailableGames { get; }
        IList<string> AvailableGameNames { get; }
        GameId GetIdFromName(string gameName);
        IGameManager LoadGameManager(GameId gameId);
        IFileHandler<ISplits> LoadSplitsFile(FileInfo filePath);
        IFileHandler<ISplits> NewSplitsFile();
    }
}
