using System;
using System.Collections.Generic;
using System.Diagnostics;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.Hook.Impl
{
    public abstract class Kernel32BaseHookStrategy : IHookStrategy
    {
        private ICollection<string> _processNames;
        
        protected ICollection<string> ProcessNames {
            get { return _processNames; }
        }

        protected IKernel32MemoryReader MemoryReader { get; private set; }

        protected IGameProcess HookedProcess { get; private set; }

        public Kernel32BaseHookStrategy(ICollection<string> processNames, IKernel32MemoryReader memoryReader)
        {
            _processNames = processNames;
            MemoryReader = memoryReader;
        }

        public abstract long GetCurrentScore();

        public bool GameIsRunning()
        {
            try {
                GetFirstRunningProcess(ProcessNames, MemoryReader);
                return true;
            }
            catch {
                return false;
            }
        }

        public bool IsHooked {
            get {
                return HookedProcess != null && !HookedProcess.HasExited;
            }
        }

        public virtual void Hook()
        {
            HookedProcess = null;
            HookedProcess = GetFirstRunningProcess(ProcessNames, MemoryReader);
        }

        private static IGameProcess GetFirstRunningProcess(ICollection<string> processNames, IKernel32MemoryReader memoryReader)
        {
            foreach (string name in processNames) {
                IGameProcess[] process = memoryReader.GetProcessesByName(name);
                if (process.Length != 0) {
                    return process[0];
                }
            }
            throw new InvalidOperationException("Game is not running.");
        }

        public void Unhook()
        {
            HookedProcess = null;
        }
    }
}
