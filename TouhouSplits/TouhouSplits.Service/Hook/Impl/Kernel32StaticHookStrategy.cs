using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.Hook.Impl
{
    public class Kernel32StaticHookStrategy : Kernel32BaseHookStrategy
    {
        private EncodingEnum _encoding;
        private int _length;
        private int _memoryAddress;

        public Kernel32StaticHookStrategy(IKernel32HookConfig config, IKernel32MemoryReader memoryReader)
            : base(config.ProcessNames, memoryReader)
        {
            _encoding = config.Encoding;
            _length = config.Length;
            _memoryAddress = config.Address;
        }

        public override long GetCurrentScore()
        {
            if (!IsHooked) {
                Hook();
            }

            if (_encoding == EncodingEnum.int32) {
                return MemoryReader.ReadInt(HookedProcess, _memoryAddress);
            }
            if (_encoding == EncodingEnum.int64) {
                return MemoryReader.ReadLong(HookedProcess, _memoryAddress);
            }
            return MemoryReader.ReadArrayOfNumbers(HookedProcess, _memoryAddress, _length);
        }
    }
}
