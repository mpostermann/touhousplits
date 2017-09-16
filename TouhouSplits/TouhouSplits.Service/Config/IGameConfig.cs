using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public interface IGameConfig
    {
        string GameName { get; }
        byte ScoreMemoryLocation { get; }
        XElement HookConfig { get; }
    }
}
