using System;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook.Reader
{
    public interface IKernel32MemoryReader
    {
        IGameProcess[] GetProcessesByName(string name);
        IntPtr ProcessHandle(int dwProcessId);
        int ReadInt(IGameProcess process, int memoryAddress);
        long ReadLong(IGameProcess proces, int memoryAddress);
    }
}