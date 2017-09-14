using System;
using System.Collections.Generic;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.Service
{
    public class SplitsFacade
    {
        public const string DEFAULT_FILE_EXT = ".tsf";

        public IList<string> AvailableGames
        {
            get {
                throw new NotImplementedException();
            }
        }
        
        public IGameManager LoadGameManager(string gameName)
        {
            throw new NotImplementedException();
        }
        
    }
}
