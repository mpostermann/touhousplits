using System;
using System.IO;
using System.IO.Pipes;

namespace TouhouSplits.Service.Hook.Impl
{
    public class MameHookStrategy : IHookStrategy
    {
        private bool _isConnected = false;
        private NamedPipeClientStream _pipeClient;
        private StreamReader _pipeReader;
        private StreamWriter _pipeWriter;

        private void Connect()
        {
            try {
                _pipeClient = new NamedPipeClientStream(".", @"\\.\pipe\luawinapi", PipeDirection.InOut, PipeOptions.None);
                _pipeClient.Connect(1000);
                _pipeReader = new StreamReader(_pipeClient);
                _pipeWriter = new StreamWriter(_pipeClient);

                _isConnected = true;
            }
            catch (Exception e) {
                Disconnect();
                throw;
            }
        }

        private void Disconnect()
        {
            _pipeClient?.Dispose();
            _pipeClient = null;

            _pipeReader?.Dispose();
            _pipeReader = null;

            _pipeWriter?.Dispose();
            _pipeWriter = null;

            _isConnected = false;
        }

        public bool GameIsRunning()
        {
            try {
                Connect();
                return _isConnected;
            }
            catch {
                return false;
            }
        }

        public void Hook()
        {
            if (!_isConnected) {
                Connect();
                IsHooked = true;
            }
        }

        public void Unhook()
        {
            Disconnect();
            IsHooked = false;
        }

        public bool IsHooked { get; private set; }

        public long GetCurrentScore()
        {
            if (!IsHooked) {
                Hook();
            }

            _pipeWriter.WriteLine("ping");
            _pipeWriter.Flush();

            return long.Parse(_pipeReader.ReadLine());
        }
    }
}
