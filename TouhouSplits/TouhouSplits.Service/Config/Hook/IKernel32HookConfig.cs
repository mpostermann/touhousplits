using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Config.Hook
{
    public interface IKernel32HookConfig
    {
        int Address { get; }
        EncodingEnum Encoding { get; }
        int Length { get; }
        string[] ProcessNames { get; }
    }
}
