using System;
using System.Collections.Generic;
using System.Threading;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Managers.SplitsBuilder;

namespace TouhouSplits.UI.Model
{
    public class PersonalBestTracker : ModelBase
    {
        private ISplitsFacade _facade;
        private IGameManager _gameManager;
        private ISplitsBuilder _personalBestBuilder;
        private Timer _timer;

        public PersonalBestTracker(ISplitsFacade facade)
        {
            _facade = facade;
            IsPolling = false;
        }

        public bool IsPersonalBestLoaded()
        {
            return _gameManager != null;
        }

        public void LoadPersonalBest(GameId gameId, string splitsName, ISplitsBuilder personalBestSplits)
        {
            if (_gameManager == null || _gameManager.Id != gameId) {
                _gameManager = _facade.LoadGameManager(gameId);
            }

            GameName = _gameManager.GameName;
            SplitsName = splitsName;
            _personalBestBuilder = personalBestSplits;
            NotifyPropertyChanged("RecordingSplits");
        }

        private string _gameName;
        public string GameName {
            get { return _gameName; }
            private set {
                _gameName = value;
                NotifyPropertyChanged("GameName");
            }
        }

        private string _splitsName;
        public string SplitsName {
            get { return _splitsName; }
            private set {
                _splitsName = value;
                NotifyPropertyChanged("SplitsName");
            }
        }

        public IList<IFileHandler<ISplits>> FavoriteSplits()
        {
            if (!IsPersonalBestLoaded()) {
                throw new InvalidOperationException("A personal best splits must be loaded before related FavoriteSplits can be retrieved.");
            }
            return _gameManager.FavoriteSplits;
        }

        private bool _isPolling;
        public bool IsPolling {
            get { return _isPolling; }
            private set {
                _isPolling = value;
                NotifyPropertyChanged("IsPolling");
            }
        }

        public IList<IPersonalBestSegment> RecordingSplits {
            get {
                if (_personalBestBuilder == null) {
                    return null;
                }
                return _personalBestBuilder.Segments;
            }
        }

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

            if (!IsPersonalBestLoaded()) {
                throw new InvalidOperationException("A personal best splits must be loaded before polling can start.");
            }

            _personalBestBuilder.Reset();
            _gameManager.Hook.Hook();

            // Set a poller to check the updated score
            _timer = new Timer(
                (param) => UpdateCurrentRecordingScore(),
                null,
                0,
                50
            );
            IsPolling = true;
        }

        private void UpdateCurrentRecordingScore()
        {
            NotifyPropertyChanged("CurrentScore");
            _personalBestBuilder.SetScoreForCurrentSegment(CurrentScore);
        }

        public void SplitToNextSegment()
        {
            if (!IsPolling) {
                return;
            }
            _personalBestBuilder.SetScoreForCurrentSegment(CurrentScore);
            if (_personalBestBuilder.CurrentSegment < _personalBestBuilder.Segments.Count - 1) {
                _personalBestBuilder.SplitToNextSegment();
            }
            else {
                StopScorePoller();
            }
        }

        public ISplits StopScorePoller()
        {
            if (!IsPolling) {
                if (_personalBestBuilder == null) {
                    return null;
                }
                return _personalBestBuilder.GetOutput();
            }

            _gameManager.Hook.Unhook();
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
            IsPolling = false;

            return _personalBestBuilder.GetOutput();
        }
    }
}
