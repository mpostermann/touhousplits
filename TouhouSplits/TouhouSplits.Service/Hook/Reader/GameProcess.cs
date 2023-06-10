using System;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook.Reader
{
    public class GameProcess : IGameProcess, IDisposable
    {
        private Process _process;

        public GameProcess(Process p) {
            _process = p;
        }

        public int Id => _process.Id;

        public bool HasExited => _process.HasExited;

        public IntPtr BaseAddress => _process.MainModule.BaseAddress;

        public IntPtr ThreadStack0Address => new IntPtr(ProcessUtils32.GetThreadStack0(_process));

        public void Dispose() {
            if (_process != null) {
                _process.Dispose();
            }
            _process = null;
        }
    }
}
