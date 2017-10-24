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
        private long _initialPollingScore;
        private Timer _timer;

        public PersonalBestTracker(ISplitsFacade facade)
        {
            _facade = facade;
            _initialPollingScore = -1;
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
            _initialPollingScore = -1;
            NotifyPropertyChanged("RecordingSplits");
            NotifyPropertyChanged("CurrentScore");
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
                NotifyPropertyChanged("CurrentScore");
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
                    long score = _gameManager.GetCurrentScore();
                    if (!GameHasStartedRun(score)) {
                        return 0;
                    }
                    return score;
                }
                else if (_personalBestBuilder != null) {
                    var score = _personalBestBuilder.GetOutput().EndingSegment.Score;
                    if (score > 0) {
                        return score;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// In some games (for example Touhou 6), after finishing a run and returning to the main menu, the score in memory will
        /// still read your score from the previous run. This method determines whether a new run has started by checking whether
        /// this score has changed or not.
        /// </summary>
        private bool GameHasStartedRun(long currentScore)
        {
            return currentScore != _initialPollingScore;
        }

        public bool IsNewPersonalBest {
            get {
                if (_personalBestBuilder == null) {
                    return false;
                }
                return _personalBestBuilder.IsNewPersonalBest();
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

            if (!_gameManager.GameIsRunning()) {
                throw new InvalidOperationException("Game is not running.");
            }

            _personalBestBuilder.Reset();
            _initialPollingScore = _gameManager.GetCurrentScore();

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
            if (!_gameManager.GameIsRunning()) {
                StopScorePoller();
            }
            else {
                NotifyPropertyChanged("CurrentScore");
                NotifyPropertyChanged("IsNewPersonalBest");
                try {
                    _personalBestBuilder.SetScoreForCurrentSegment(CurrentScore);
                }
                catch (InvalidOperationException) {
                    StopScorePoller();
                }
            }
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

            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
            IsPolling = false;

            return _personalBestBuilder.GetOutput();
        }
    }
}
