using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Managers.Splits;

namespace TouhouSplits.Service.Managers.Game
{
    public interface IGameManager
    {
        string GameName { get; }
        ISplitsManager SplitsManager { get; }
        IHookStrategy Hook { get; }
    }
}
