using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class RecentSplitsViewModel : ViewModelBase
    {
        private SplitsFacade _splitsFacade;
        
        public ICommand RemoveSplits { get; private set; }

        public RecentSplitsViewModel(SplitsFacade facade, ISplitsFile splitsFile)
        {
            _splitsFacade = facade;
            if (splitsFile != null) {
                CurrentGame = splitsFile.Splits.GameName;
            }
        }

        public ISplitsFile SelectedSplits { get; private set; }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
            }
        }

        private string _currentGame;
        public string CurrentGame {
            get {
                return _currentGame;
            }
            set {
                _currentGame = value;
                RecentSplits = _splitsFacade.LoadGameManager(_currentGame).SplitsManager.RecentSplits;
                NotifyPropertyChanged("CurrentGame");
            }
        }

        private IList<ISplitsFile> _recentSplits;
        public IList<ISplitsFile> RecentSplits {
            get {
                return _recentSplits; 
            }
            set {
                _recentSplits = value;
                NotifyPropertyChanged("RecentSplits");
            }
        }
    }
}
