using System;
using System.Collections.Generic;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.Service
{
    public class SplitsFacade
    {
        IList<string> AvailableGames
        {
            get {
                throw new NotImplementedException();
            }
        }
        
        IGameManager LoadGameManager(string gameName)
        {
            throw new NotImplementedException();
        }
        
    }
}
