using System.Collections.Generic;
using System.IO;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.Service
{
    public interface ISplitsFacade
    {
        IList<string> AvailableGames { get; }
        IGameManager LoadGameManager(string gameName);
        IFileHandler<ISplits> LoadSplitsFile(FileInfo filePath);
        IFileHandler<ISplits> NewSplitsFile();
    }
}
