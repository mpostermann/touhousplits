using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace TouhouSplits.Service.Hook
{
    public class Kernel32HookStrategy : IHookStrategy
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private int _address;
        private EncodingEnum _encoding;
        private string[] _processNames;
        private IntPtr _processHandle;
        private bool _isHooked;

        public Kernel32HookStrategy(XElement configElement)
        {
            _address = ParseAddress(configElement);
            _encoding = ParseEncoding(configElement);
            _processNames = ParseProcessNames(configElement);
            _isHooked = false;
        }

        private static int ParseAddress(XElement configElement)
        {
            return int.Parse(configElement.Attribute("address").Value);
        }

        private static EncodingEnum ParseEncoding(XElement configElement)
        {
            string parsedEncoding = configElement.Attribute("encoding").Value.Trim().ToLower();
            switch (parsedEncoding) {
                case "int32":
                    return EncodingEnum.int32;
                case "int64":
                    return EncodingEnum.int64;
                default:
                    throw new NotSupportedException(string.Format("Encoding type \"{0}\" is not supported", parsedEncoding));
            }
        }

        private static string[] ParseProcessNames(XElement configElement)
        {
            return configElement.Attribute("process").Value.Split('|');
        }

        public long GetCurrentScore()
        {
            int bytesRead = 0;
            byte[] buffer;
            if (_encoding == EncodingEnum.int32) {
                buffer = new byte[4];
            }
            else {
                buffer = new byte[8];
            }

            ReadProcessMemory((int)_processHandle, _address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public void Hook()
        {
            if (_isHooked) {
                return;
            }
            Process process = GetFirstRunningProcess(_processNames);
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
