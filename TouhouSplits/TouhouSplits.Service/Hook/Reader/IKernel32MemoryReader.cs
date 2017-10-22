using System;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook.Reader
{
    public interface IKernel32MemoryReader
    {
        Process[] GetProcessesByName(string name);
        IntPtr ProcessHandle(int dwProcessId);
        int ReadInt(Process process, int memoryAddress);
        long ReadLong(Process proces, int memoryAddress);
    }
}