using System;

namespace TouhouSplits.Service.Config
{
    public interface IGameConfig
    {
        string GameName { get; }
        byte ScoreMemoryLocation { get; }
        Type HookStrategy { get; }
    }
}
