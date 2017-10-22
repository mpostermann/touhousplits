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

            int nextAddress = IntPtr.Add(HookedProcess.BaseAddress, _memoryAddress).ToInt32();

            int i = 0;
            do {
                nextAddress = MemoryReader.ReadInt(HookedProcess, nextAddress);

                if (i < _pointerOffsets.Length) {
                    nextAddress += _pointerOffsets[i];
                    i++;
                }
            }
            while (i < _pointerOffsets.Length);

            if (_encoding == EncodingEnum.int32) {
                return MemoryReader.ReadInt(HookedProcess, nextAddress);
            }
            else {
                return MemoryReader.ReadLong(HookedProcess, nextAddress);
            }
        }
    }
}
