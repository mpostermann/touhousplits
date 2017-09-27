using System;
using System.Collections.Generic;
using TouhouSplits.MVVM;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.UI.Model
{
    public class RecentSplitsModel : ModelBase
    {
        private IGameManager _gameManager;

        public RecentSplitsModel(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public IList<ISplitsFile> RecentSplits {
            get { return _gameManager.RecentSplits; }
        }

        public void RemoveSplitsFromRecentSplits()
        {
            throw new NotImplementedException();
        }
    }
}
