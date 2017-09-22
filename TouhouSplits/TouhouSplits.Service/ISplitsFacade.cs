using System.Collections.Generic;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.Service
{
    public interface ISplitsFacade
    {
        IList<string> AvailableGames { get; }
        IGameManager LoadGameManager(string gameName);
    }
}
