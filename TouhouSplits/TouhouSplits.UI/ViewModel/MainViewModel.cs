using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SplitsFacade _splitsFacade;
        private IGameManager _currentGame;
        private ISplits _currentSplits;
        private ISplits _recordingSplits;

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand RecentSplitsCommand { get; private set; }
        public ICommand NextSplitCommand { get; private set; }
        public ICommand PreviousSplitCommand { get; private set; }
        public ICommand StartRecordingSplitCommand { get; private set; }
        public ICommand StopRecordingSplitCommand { get; private set; }

        public MainViewModel()
        {
            _splitsFacade = new SplitsFacade();
            NewSplitCommand = new RelayCommand(() => NewSplit());
            EditSplitCommand = new RelayCommand(() => EditSplit());
            RecentSplitsCommand = new RelayCommand(() => RecentSplits());
        }
        
        public ISplits CurrentSplits {
            get { return _currentSplits; }
            set {
                _currentSplits = value;
                NotifyPropertyChanged("CurrentSplits");
            }
        }

        public ISplits RecordingSplits {
            get { return _recordingSplits; }
            set {
                _recordingSplits = value;
                NotifyPropertyChanged("RecordingSplits");
            }
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            //Todo: instantiate a new Segment
            ISplits newSplit = null;
            loadSplitView.DataContext = new EditSplitsViewModel(newSplit);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                CurrentSplits = newSplit;
                _currentGame = _splitsFacade.LoadGameManager(newSplit.GameName);
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(CurrentSplits);
            loadSplitView.ShowDialog();

            /* Reload the current game if the parent game was edited */
            if (CurrentSplits.GameName != _currentGame.GameName) {
                _currentGame = _splitsFacade.LoadGameManager(CurrentSplits.GameName);
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                CurrentSplits = rsViewModel.SelectedSplits;
                _currentGame = _splitsFacade.LoadGameManager(CurrentSplits.GameName);
            }
        }
    }
}
