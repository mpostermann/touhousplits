using System.IO;
using System.Xml.Linq;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Config
{
    public interface IGameConfig
    {
        GameId Id { get; }
        string GameName { get; }
        FileInfo FavoriteSplitsList { get; }
        XElement HookConfig { get; }
    }
}
