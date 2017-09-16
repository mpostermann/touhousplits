using System.Collections.Generic;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public interface IGameConfig
    {
        string GameName { get; }
        IList<string> Processes { get; }
        byte ScoreMemoryLocation { get; }
        XElement HookConfig { get; }
    }
}
