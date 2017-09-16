using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public string GameName { get; private set; }
        public byte ScoreMemoryLocation { get; private set; } 
        public XElement HookConfig { get; private set; }

        private List<string> _processes;
        public IList<string> Processes { get { return _processes.AsReadOnly(); } }

        public GameConfig(XElement configElement)
        {
            throw new NotImplementedException();
        }
    }
}
