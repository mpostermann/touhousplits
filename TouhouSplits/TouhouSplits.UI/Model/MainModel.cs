using System;
using System.Collections.Generic;
using System.Threading;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.UI.Model
{
    public class MainModel : ModelBase
    {
        private ISplitsFacade _facade;
        private IGameManager _gameManager;
        private Timer _timer;

        public MainModel(ISplitsFacade facade)
        {
            _facade = facade;
            IsPolling = false;
        }

        private IFileHandler<ISplits> _currentSplitsFile;
        public IFileHandler<ISplits> CurrentSplitsFile {
            get { return _currentSplitsFile; }
            set {
                if (_currentSplitsFile != null) {
                    _currentSplitsFile.Close();
                }
                _currentSplitsFile = value;

                if (_gameManager == null || _gameManager.GameName != _currentSplitsFile.Object.GameName) {
                    _gameManager = _facade.LoadGameManager(_currentSplitsFile.Object.GameName);
                }

                NotifyPropertyChanged("CurrentSplitsFile");
                NotifyPropertyChanged("FavoriteSplits");
            }
        }

        private ISplits _recordingSplits;
        public ISplits RecordingSplits {
            get {
                return _recordingSplits;
            }
            set {
                _recordingSplits = value;
                NotifyPropertyChanged("RecordingSplits");
            }
        }

        public IList<IFileHandler<ISplits>> FavoriteSplits { get { return _gameManager.FavoriteSplits; } }

        public bool IsPolling { get; private set; }

        public long CurrentScore {
            get {
                if (IsPolling) {
                    return _gameManager.Hook.GetCurrentScore();
                }
                return -1;
            }
        }

        public void StartScorePoller()
        {
            if (IsPolling) {
                return;
            }

            if (_gameManager == null) {
                throw new InvalidOperationException("Game must be set before polling can start.");
            }

            _gameManager.Hook.Hook();

            // Set a poller to check the updated score
            _timer = new Timer(
                (param) => NotifyPropertyChanged("CurrentScore"),
                null,
                0,
                50
            );
            IsPolling = true;
        }

        public void StopScorePoller()
        {
            if (!IsPolling) {
                return;
            }

            _gameManager.Hook.Unhook();
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
            IsPolling = false;
        }
    }
}
