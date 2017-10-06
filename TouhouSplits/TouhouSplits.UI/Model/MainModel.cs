using System;
using System.Collections.Generic;
using System.Threading;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Managers.SplitsBuilder;

namespace TouhouSplits.UI.Model
{
    public class MainModel : ModelBase
    {
        private ISplitsFacade _facade;
        private IGameManager _gameManager;
        private ISplitsBuilder _personalBestBuilder;
        private Timer _timer;

        public MainModel(ISplitsFacade facade)
        {
            _facade = facade;
            IsPolling = false;
        }

        public void ResetModel(ISplits personalBestSplits)
        {
            if (_gameManager == null || _gameManager.GameName != personalBestSplits.GameName) {
                _gameManager = _facade.LoadGameManager(personalBestSplits.GameName);
            }

            _personalBestBuilder = new PersonalBestSplitsBuilder(personalBestSplits);
            NotifyPropertyChanged("RecordingSplits");
        }

        public IList<IFileHandler<ISplits>> FavoriteSplits()
        {
            if (_gameManager == null) {
                throw new InvalidOperationException("A personal best splits must be loaded before related FavoriteSplits can be retrieved.");
            }
            return _gameManager.FavoriteSplits;
        }

        public bool IsPolling { get; private set; }

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

            if (_gameManager == null) {
                throw new InvalidOperationException("Game must be set before polling can start.");
            }

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
