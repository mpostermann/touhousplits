using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TouhouSplits.Service.Config.Hook;

namespace TouhouSplits.Service.Hook.Impl
{
    public class Kernel32HookStrategy : Kernel32BaseHookStrategy
    {
        private EncodingEnum _encoding;
        private int _memoryAddress;

        public Kernel32HookStrategy(IKernel32HookConfig config, IKernel32MemoryReader memoryReader)
            : base(config.ProcessNames, memoryReader)
        {
            _encoding = config.Encoding;
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
            else {
                return MemoryReader.ReadLong(HookedProcess, _memoryAddress);
            }
        }
    }
}
