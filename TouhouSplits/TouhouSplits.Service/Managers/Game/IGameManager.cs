using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        string GameName { get; }
        IHookStrategy Hook { get; }
        IList<ISplitsFile> RecentSplits { get; }
        void AddToRecentSplits(ISplitsFile splitsFile);
    }
}
