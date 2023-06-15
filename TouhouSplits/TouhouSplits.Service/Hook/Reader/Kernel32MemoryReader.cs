using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TouhouSplits.Service.Hook.Reader
{
    public class Kernel32MemoryReader : IKernel32MemoryReader
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public IGameProcess[] GetProcessesByName(string name) {
            var processes = Process.GetProcessesByName(name);
            int count = 0;
            if (processes != null) {
                count = processes.Length;
            }

            IGameProcess[] gameProcesses = new GameProcess[count];
            for (int i = 0; i < count; i++) {
                gameProcesses[0] = new GameProcess(processes[0]);
            }
            return gameProcesses;
        }

        public IntPtr ProcessHandle(int dwProcessId) {
            return OpenProcess(PROCESS_WM_READ, false, dwProcessId);
        }

        public int ReadInt(IGameProcess process, int memoryAddress) {
            byte[] buffer = ReadBytes(process, memoryAddress, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadLong(IGameProcess process, int memoryAddress) {
            byte[] buffer = ReadBytes(process, memoryAddress, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public long ReadArrayOfNumbers(IGameProcess process, int memoryAddress, int length)
        {
            if (length <= 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "length must be greater than 0");
            }

            long value = 0;
            for (int i = 0; i < length; i++) {
                byte digit = ReadBytes(process, memoryAddress, 1)[0];
                memoryAddress++;

                value *= 10;
                value += digit;
            }

            return value;
        }

        private byte[] ReadBytes(IGameProcess process, int memoryAddress, int numBytes) {
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
