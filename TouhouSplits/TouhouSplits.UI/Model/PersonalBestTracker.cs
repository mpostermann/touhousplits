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
            _initialPollingScore = Constants.UNSET_SCORE;
            IsPolling = false;
        }

        public bool IsPersonalBestLoaded()
        {
            return _gameManager != null;
        }

        public void LoadPersonalBest(GameId gameId, string splitsName, ISplitsBuilder personalBestSplits)
        {
            StopScorePoller();
            if (_gameManager == null || _gameManager.Id != gameId) {
                _gameManager = _facade.LoadGameManager(gameId);
            }

            GameName = _gameManager.GameName;
            IsHookable = _gameManager.GameIsHookable;
            SplitsName = splitsName;
            _personalBestBuilder = personalBestSplits;
            _initialPollingScore = Constants.UNSET_SCORE;
            NotifyPropertyChanged("RecordingSplits");
            NotifyPropertyChanged("CurrentScore");
        }

        public bool HasError {
            get { return _lastException != null; }
        }

        private Exception _lastException;
        public Exception LastError {
            get { return _lastException; }
            set {
                _lastException = value;
                NotifyPropertyChanged("LastError");
                NotifyPropertyChanged("HasError");
            }
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

        private bool _isHookable;
        public bool IsHookable {
            get { return _isHookable; }
            private set {
                _isHookable = value;
                NotifyPropertyChanged("IsHookable");
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
                if (_personalBestBuilder != null) {
                    var score = _personalBestBuilder.GetOutput().EndingSegment.Score;
                    if (score >= 0) {
                        return score;
                    }
                }
                return Constants.UNSET_SCORE;
            }
        }

        /// <summary>
        /// In some games (for example Touhou 6), after finishing a run and returning to the main menu, the score in memory will
        /// still read your score from the previous run. This method determines whether a new run has started by checking whether
        /// this score has changed or not.
        /// </summary>
        private bool GameHasStartedRun(long currentScore) {
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

        public void ClearError()
        {
            LastError = null;
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

            try {
                _personalBestBuilder.Reset();
                _initialPollingScore = _gameManager.GetCurrentScore();
                _personalBestBuilder.MarkAsStarted();
            }
            catch (Exception e) {
                LastError = e;
                return;
            }

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
            try {
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
                        /* An InvalidOperationException is thrown if the game process dies while polling.
                         * This isn't considered an error, so simply stop polling without tracking the exception. */
                        StopScorePoller();
                    }
                }
            }
            catch (Exception e) {
                LastError = e;
                StopScorePoller();
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
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }

            IsPolling = false;
            if (_personalBestBuilder == null) {
                return null;
            }
            
            // End the current segment before closing out the run
            _personalBestBuilder.MarkAsStopped();

            return _personalBestBuilder.GetOutput();
        }
    }
}
