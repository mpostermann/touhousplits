
namespace TouhouSplits.Service.Hook
{
    public interface IHookStrategy
    {
        void Hook();
        void Unhook();
        long GetCurrentScore();
    }
}
