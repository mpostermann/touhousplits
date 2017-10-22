using System;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook
{
    public interface IKernel32MemoryReader
    {
        IntPtr ProcessHandle(int dwProcessId);
        int ReadInt(Process process, int memoryAddress);
        long ReadLong(Process proces, int memoryAddress);
    }
}