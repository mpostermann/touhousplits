using System.Collections.Generic;
using System.Threading;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Managers.Splits;

namespace TouhouSplits.UI.Model
{
    public class Game : ModelBase
    {
        private SplitsFacade _facade;
        private IGameManager _gameManager;
        private Timer _timer;

        public string GameName {
            get {
                return _gameManager.GameName;
            }
            set {
                _gameManager = _facade.LoadGameManager(value);
                NotifyPropertyChanged("GameManager");
                NotifyPropertyChanged("RecentSplits");
            }
        }

        public ISplitsManager SplitsManager { get { return _gameManager.SplitsManager; } }
        public IList<ISplitsFile> RecentSplits { get { return _gameManager.SplitsManager.RecentSplits; } }
        public bool IsPolling { get; private set; }

        public long CurrentScore {
            get {
                if (IsPolling) {
                    return _gameManager.Hook.GetCurrentScore();
                }
                return -1;
            }
        }

        public Game(SplitsFacade facade, IGameManager manager)
        {
            _facade = facade;
            _gameManager = manager;
            IsPolling = false;
        }

        public void StartScorePoller()
        {
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
            _gameManager.Hook.Unhook();
            _timer.Dispose();
            _timer = null;
            IsPolling = false;
        }
    }
}
