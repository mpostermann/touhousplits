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
        ISplitsFile DeserializeSplits(FileInfo filepath);
        ISplitsFile SerializeSplits(ISplits splits, FileInfo filepath);
    }
}
