using System;
using System.Diagnostics;

namespace TouhouSplits.Service.Hook.Reader
{
    public class GameProcess : IGameProcess, IDisposable
    {
        private Process _process;

        public GameProcess(Process p)
        {
            _process = p;
        }

        public int Id {
            get {
                return _process.Id;
            }
        }

        public bool HasExited {
            get {
                return _process.HasExited;
            }
        }

        public void Dispose()
        {
            if (_process != null) {
                _process.Dispose();
            }
            _process = null;
        }
    }
}
