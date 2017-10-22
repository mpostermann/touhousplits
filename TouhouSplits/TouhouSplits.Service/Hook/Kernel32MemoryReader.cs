using System;
using System.Diagnostics;
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

        public int ReadInt(Process process, int memoryAddress)
        {
            byte[] buffer = ReadBytes(process, memoryAddress, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadLong(Process process, int memoryAddress)
        {
            byte[] buffer = ReadBytes(process, memoryAddress, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        private byte[] ReadBytes(Process process, int memoryAddress, int numBytes)
        {
            if (process == null || process.HasExited) {
                throw new InvalidOperationException("Game is not running");
            }

            byte[] buffer = new byte[numBytes];
            int bytesRead = 0;
            IntPtr processHandle = this.ProcessHandle(process.Id);
            if (!ReadProcessMemory((int)processHandle, memoryAddress, buffer, buffer.Length, ref bytesRead)) {
                throw new InvalidOperationException("Read process memory failed");
            }
            return buffer;
        }
    }
}
