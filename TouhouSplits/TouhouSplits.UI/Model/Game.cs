using System.Collections.Generic;
using System.Threading;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.UI.Model
{
    public class Game : ModelBase
    {
        private ISplitsFacade _facade;
        private Timer _timer;

        public Game(ISplitsFacade facade, IGameManager manager)
        {
            _facade = facade;
            GameManager = manager;
            IsPolling = false;
        }

        public string GameName {
            get {
                return GameManager.GameName;
            }
            set {
                GameManager = _facade.LoadGameManager(value);
                NotifyPropertyChanged("RecentSplits");
            }
        }

        public IGameManager GameManager { get; private set; }
        public IList<ISplitsFile> RecentSplits { get { return GameManager.RecentSplits; } }
        public bool IsPolling { get; private set; }

        public long CurrentScore {
            get {
                if (IsPolling) {
                    return GameManager.Hook.GetCurrentScore();
                }
                return -1;
            }
        }

        public void StartScorePoller()
        {
            GameManager.Hook.Hook();

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
            GameManager.Hook.Unhook();
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
            IsPolling = false;
        }
    }
}
