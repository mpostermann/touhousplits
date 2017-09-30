using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        string GameName { get; }
        IHookStrategy Hook { get; }
        IList<IFileHandler<ISplits>> FavoriteSplits { get; }
        void AddOrUpdateFavorites(IFileHandler<ISplits> splitsFile);
    }
}
