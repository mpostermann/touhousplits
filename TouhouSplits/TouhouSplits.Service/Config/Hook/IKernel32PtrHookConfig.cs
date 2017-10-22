
namespace TouhouSplits.Service.Config.Hook
{
    public interface IKernel32PtrHookConfig : IKernel32HookConfig
    {
        int[] PointerOffsets { get; }
    }
}
