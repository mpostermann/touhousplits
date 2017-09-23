﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TouhouSplits.Service.Config.Hook;

namespace TouhouSplits.Service.Hook.Impl
{
    public class Kernel32HookStrategy : IHookStrategy
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        IKernel32HookConfig _config;
        private IntPtr _processHandle;
        private bool _isHooked;

        public Kernel32HookStrategy(IKernel32HookConfig config)
        {
            _config = config;
            _isHooked = false;
        }

        public long GetCurrentScore()
        {
            int bytesRead = 0;
            byte[] buffer;
            if (_config.Encoding == EncodingEnum.int32) {
                buffer = new byte[4];
            }
            else {
                buffer = new byte[8];
            }

            ReadProcessMemory((int)_processHandle, _config.Address, buffer, buffer.Length, ref bytesRead);

            if (_config.Encoding == EncodingEnum.int32) {
                return BitConverter.ToInt32(buffer, 0);
            }
            else {
                return BitConverter.ToInt64(buffer, 0);
            }
        }

        public void Hook()
        {
            if (_isHooked) {
                return;
            }
            Process process = GetFirstRunningProcess(_config.ProcessNames);
            _processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            _isHooked = true;
        }

        private static Process GetFirstRunningProcess(string[] processNames)
        {
            foreach (string name in processNames) {
                Process[] process = Process.GetProcessesByName(name);
                if (process.Length != 0) {
                    return process[0];
                }
            }
            throw new InvalidOperationException("Process is not running.");
        }

        public void Unhook()
        {
            _isHooked = false;
        }
    }
}