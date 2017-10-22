using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook.Impl
{
    public abstract class Kernel32BaseHookStrategy : IHookStrategy
    {
        private ICollection<string> _processNames;
        
        protected ICollection<string> ProcessNames {
            get { return _processNames; }
        }

        protected IKernel32MemoryReader MemoryReader { get; private set; }

        protected IntPtr ProcessHandle { get; private set; }

        public Kernel32BaseHookStrategy(ICollection<string> processNames, IKernel32MemoryReader memoryReader)
        {
            _processNames = processNames;
            MemoryReader = memoryReader;
            IsHooked = false;
        }

        public abstract long GetCurrentScore();

        public bool GameIsRunning()
        {
            try {
                GetFirstRunningProcess(ProcessNames);
                return true;
            }
            catch {
                return false;
            }
        }

        public bool IsHooked { get; private set; }

        public virtual void Hook()
        {
            if (IsHooked) {
                return;
            }
            Process process = GetFirstRunningProcess(ProcessNames);
            ProcessHandle = MemoryReader.ProcessHandle(process.Id);
            IsHooked = true;
        }

        private static Process GetFirstRunningProcess(ICollection<string> processNames)
        {
            foreach (string name in processNames) {
                Process[] process = Process.GetProcessesByName(name);
                if (process.Length != 0) {
                    return process[0];
                }
            }
            throw new InvalidOperationException("Game is not running.");
        }

        public void Unhook()
        {
            IsHooked = false;
        }
    }
}
