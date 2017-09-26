using System.Collections.Generic;
using System.IO;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        string GameName { get; }
        IHookStrategy Hook { get; }
        IList<ISplitsFile> RecentSplits { get; }
        ISplitsFile SerializeSplits(ISplits splits, FileInfo filePath);
        ISplitsFile DeserializeSplits(FileInfo filePath);
    }
}
