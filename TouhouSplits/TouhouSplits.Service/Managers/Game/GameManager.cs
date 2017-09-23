using TouhouSplits.MVVM;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Managers.Splits;

namespace TouhouSplits.Service.Managers.Game
{
    public class GameManager : ModelBase, IGameManager
    {
        private IGameConfig _config;

        public string GameName { get { return _config.GameName; } } 
        public ISplitsManager SplitsManager { get; private set; } 
        public IHookStrategy Hook { get; private set; } 

        public GameManager(IGameConfig config)
        {
            _config = config;
            Hook = new Kernel32HookStrategy(config.HookConfig);
        }
    }
}
