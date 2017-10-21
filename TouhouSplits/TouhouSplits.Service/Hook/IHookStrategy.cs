
namespace TouhouSplits.Service.Hook
{
    public interface IHookStrategy
    {
        bool GameIsRunning();
        void Hook();
        void Unhook();
        bool IsHooked { get; }
        long GetCurrentScore();
    }
}
