using System;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public string GameName { get; private set; } 
        public byte ScoreMemoryLocation { get; private set; } 
        public Type HookStrategy { get; private set; }

        public GameConfig(XElement configElement)
        {
            throw new NotImplementedException();
        }
    }
}
