
namespace TouhouSplits.Service.Hook
{
    public interface IHookStrategy
    {
        void Hook();
        void Unhook();
        bool IsHooked { get; }
        long GetCurrentScore();
    }
}
