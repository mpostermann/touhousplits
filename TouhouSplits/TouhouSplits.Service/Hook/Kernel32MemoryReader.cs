using System;
using System.Runtime.InteropServices;

namespace TouhouSplits.Service.Hook
{
    public class Kernel32MemoryReader : IKernel32MemoryReader
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public IntPtr ProcessHandle(int dwProcessId)
        {
            return OpenProcess(PROCESS_WM_READ, false, dwProcessId);
        }

        public int ReadInt(IntPtr processHandle, int memoryAddress)
        {
            byte[] buffer = ReadBytes(processHandle, memoryAddress, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadLong(IntPtr processHandle, int memoryAddress)
        {
            byte[] buffer = ReadBytes(processHandle, memoryAddress, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        private static byte[] ReadBytes(IntPtr processHandle, int memoryAddress, int numBytes)
        {
            byte[] buffer = new byte[numBytes];
            int bytesRead = 0;
            if (!ReadProcessMemory((int)processHandle, memoryAddress, buffer, buffer.Length, ref bytesRead)) {
                throw new InvalidOperationException("Read process memory failed");
            }
            return buffer;
        }
    }
}
