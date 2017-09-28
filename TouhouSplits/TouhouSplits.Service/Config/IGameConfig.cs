using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public interface IGameConfig
    {
        string GameName { get; }
        FileInfo FavoriteSplitsList { get; }
        XElement HookConfig { get; }
    }
}
