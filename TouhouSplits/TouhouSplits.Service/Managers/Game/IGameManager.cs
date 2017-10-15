﻿using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        GameId Id { get; }
        string GameName { get; }
        IHookStrategy Hook { get; }
        IList<IFileHandler<ISplits>> FavoriteSplits { get; }
        bool FavoriteSplitsFileLoaded { get; }
        void AddOrUpdateFavorites(IFileHandler<ISplits> splitsFile);
    }
}
