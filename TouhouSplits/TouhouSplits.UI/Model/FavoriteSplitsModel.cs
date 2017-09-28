using System;
using System.Collections.Generic;
using TouhouSplits.MVVM;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.UI.Model
{
    public class FavoriteSplitsModel : ModelBase
    {
        private IGameManager _gameManager;

        public FavoriteSplitsModel(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public IList<ISplitsFile> FavoriteSplits {
            get { return _gameManager.FavoriteSplits; }
        }

        public void RemoveSplitsFromFavoriteSplits()
        {
            throw new NotImplementedException();
        }
    }
}
