using System;
using System.IO;
using System.IO.Pipes;

namespace TouhouSplits.Service.Hook.Impl
{
    public class MameHookStrategy : IHookStrategy
    {
        private long _curerntScore = 0;
        private NamedPipeClientStream _pipeClient;
        private StreamReader _pipeReader;

        public bool GameIsRunning()
        {
            try {
                Hook();
                return IsHooked;
            }
            catch {
                return false;
            }
        }

        public void Hook()
        {
            if (!IsHooked) {
                try {
                    _curerntScore = 0;
                    _pipeClient = new NamedPipeClientStream(".", @"luawinapi", PipeDirection.InOut, PipeOptions.Asynchronous);
                    _pipeClient.Connect(1000);

                    _pipeReader = new StreamReader(_pipeClient);
                }
                catch (Exception e) {
                    Unhook();
                    throw;
                }
            }
        }

        public void Unhook()
        {
            _pipeReader?.Dispose();
            _pipeReader = null;

            _pipeClient?.Dispose();
            _pipeClient = null;
        }

        public bool IsHooked => _pipeClient != null && _pipeClient.IsConnected;

        public long GetCurrentScore()
        {
            if (!IsHooked) {
                Hook();
            }

            if (long.TryParse(_pipeReader.ReadLine(), out long newScore)) {
                _curerntScore = newScore;
            }
            return _curerntScore;
        }
    }
}
