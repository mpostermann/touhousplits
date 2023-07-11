using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace TouhouSplits.Service.Hook.Impl
{
    public class MameHookStrategy : IHookStrategy
    {
        private int _isHooked; // 1 means true, 0 means false
        public bool IsHooked => _isHooked == 1;

        private long _lastScore;
        private CancellationTokenSource _scoreCancellation;
        private CancellationToken _scoreCancellationToken;
        private Task _scoreThread;

        public MameHookStrategy()
        {
            //_pipeServer = new NamedPipeServerStream("luawinapi", PipeDirection.InOut, -1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public bool GameIsRunning()
        {
            return true;
        }

        public void Hook()
        {
            if (!IsHooked) {
                try {
                    _lastScore = 0;

                    _scoreCancellation = new CancellationTokenSource();
                    _scoreCancellationToken = _scoreCancellation.Token;
                    _scoreThread = new Task(UpdateScore, _scoreCancellationToken);
                    _scoreThread.ContinueWith(t => Unhook(), TaskContinuationOptions.OnlyOnFaulted);

                    _scoreThread.Start();
                    Interlocked.Exchange(ref _isHooked, 1);
                }
                catch (Exception e) {
                    Unhook();
                    throw;
                }
            }
        }

        private void UpdateScore()
        {
            var pipeServer = new NamedPipeServerStream("luawinapi", PipeDirection.InOut, -1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            pipeServer.WaitForConnection();
            var pipeReader = new StreamReader(pipeServer);

            try {
                while (true) {
                    // Get the most recent score
                    var readTask = Task<string>.Factory.StartNew(() => {
                        string line = null;
                        while (string.IsNullOrEmpty(line)) {
                            line = pipeReader.ReadLine();
                            Thread.Sleep(10);
                        }
                        return line;
                    }, _scoreCancellationToken);

                    if (!readTask.Wait(1000)) {
                        throw new IOException("Timed out while reading from pipe.");
                    }
                    if (readTask.Exception != null) {
                        throw readTask.Exception;
                    }

                    // Update the score in a thread-safe manner
                    string result = readTask.Result;
                    if (long.TryParse(result, out var newScore)) {
                        Interlocked.Exchange(ref _lastScore, newScore);
                    }

                    _scoreCancellationToken.ThrowIfCancellationRequested();
                }
            }
            finally {
                pipeReader.Dispose();
                if (pipeServer.IsConnected) {
                    pipeServer.Disconnect();
                }
                pipeServer.Close();

                Interlocked.Exchange(ref _isHooked, 0);
            }
        }

        public void Unhook()
        {
            _scoreCancellation?.Cancel();
            _scoreThread = null;

            Interlocked.Exchange(ref _isHooked, 0);
        }

        public long GetCurrentScore()
        {
            return Interlocked.Read(ref _lastScore);
        }
    }
}
