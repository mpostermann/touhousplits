using System;

namespace TouhouSplits.Service.Hook
{
    public interface IKernel32MemoryReader
    {
        IntPtr ProcessHandle(int dwProcessId);
        int ReadInt(IntPtr processHandle, int memoryAddress);
        long ReadLong(IntPtr processHandle, int memoryAddress);
    }
}