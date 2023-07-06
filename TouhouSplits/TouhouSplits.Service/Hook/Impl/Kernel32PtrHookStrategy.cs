using System;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.Hook.Impl
{
    public class Kernel32PtrHookStrategy : Kernel32BaseHookStrategy
    {
        private EncodingEnum _encoding;
        private int _length;
        private int _memoryAddress;
        private int[] _pointerOffsets;
        private bool _useThreadStack0;

        public Kernel32PtrHookStrategy(IKernel32PtrHookConfig config, IKernel32MemoryReader memoryReader)
            : base(config.ProcessNames, memoryReader)
        {
            _encoding = config.Encoding;
            _length = config.Length;
            _memoryAddress = config.Address;
            _pointerOffsets = config.PointerOffsets;
            _useThreadStack0 = config.UseThreadStack0;
        }

        public override long GetCurrentScore()
        {
            if (!IsHooked) {
                Hook();
            }

            /* Get the starting address to evaluate the pointer from.
             * The method for this varies depending on whether the pointer starts from the process or the thread stack.
             */
            int nextAddress = IntPtr.Add(HookedProcess.BaseAddress, _memoryAddress).ToInt32();
            if (_useThreadStack0) {
                nextAddress = IntPtr.Subtract(HookedProcess.ThreadStack0Address, _memoryAddress).ToInt32();
            }

            /* Iterate through the pointer chain */
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
            if (_encoding == EncodingEnum.int64) {
                return MemoryReader.ReadLong(HookedProcess, nextAddress);
            }
            return MemoryReader.ReadArrayOfNumbers(HookedProcess, nextAddress, _length);
        }
    }
}
