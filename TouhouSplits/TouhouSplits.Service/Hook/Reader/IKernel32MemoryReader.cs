using System;

namespace TouhouSplits.Service.Hook.Reader
{
    public interface IKernel32MemoryReader
    {
        IGameProcess[] GetProcessesByName(string name);
        IntPtr ProcessHandle(int dwProcessId);
        int ReadInt(IGameProcess process, int memoryAddress);
        long ReadLong(IGameProcess process, int memoryAddress);
        long ReadArrayOfNumbers(IGameProcess process, int memoryAddress, int length);
    }
}