using System;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.Hook.Impl
{
    public class Kernel32PtrHookStrategy : Kernel32BaseHookStrategy
    {
        private EncodingEnum _encoding;
        private int _memoryAddress;
        private int[] _pointerOffsets;

        public Kernel32PtrHookStrategy(IKernel32PtrHookConfig config, IKernel32MemoryReader memoryReader)
            : base(config.ProcessNames, memoryReader)
        {
            _encoding = config.Encoding;
            _memoryAddress = config.Address;
            _pointerOffsets = config.PointerOffsets;
        }

        public override long GetCurrentScore()
        {
            if (!IsHooked) {
                Hook();
            }

            IntPtr baseAddress = IntPtr.Add(HookedProcess.BaseAddress, _memoryAddress);
            int nextAddress = MemoryReader.ReadInt(HookedProcess, baseAddress.ToInt32());

            for (int i = 0; i < _pointerOffsets.Length - 1; i++) {
                nextAddress += _pointerOffsets[i];
                nextAddress = MemoryReader.ReadInt(HookedProcess, nextAddress);
            }

            nextAddress += _pointerOffsets[_pointerOffsets.Length - 1];
            if (_encoding == EncodingEnum.int32) {
                return MemoryReader.ReadInt(HookedProcess, nextAddress);
            }
            else {
                return MemoryReader.ReadLong(HookedProcess, nextAddress);
            }
        }
    }
}
