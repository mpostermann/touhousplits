using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Config.Hook
{
    public interface IKernel32HookConfig
    {
        int Address { get; }
        EncodingEnum Encoding { get; }
        string[] ProcessNames { get; }
    }
}
