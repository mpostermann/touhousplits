using System;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public string GameName { get; private set; } 
        public byte ScoreMemoryLocation { get; private set; } 
        public XElement HookConfig { get; private set; }

        public GameConfig(XElement configElement)
        {
            throw new NotImplementedException();
        }
    }
}
