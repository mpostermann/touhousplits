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
        private ISegment _currentSegment;
        private ISegment _recordingSegment;

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

        public string CurrentGameName {
            get {
                if (CurrentSegment == null) {
                    return "Open a split!";
                }
                return CurrentSegment.ParentGameName;
            }
        }

        public string CurrentSegmentName {
            get {
                if (CurrentSegment == null) {
                    return string.Empty;
                }
                return CurrentSegment.SegmentName;
            }
        }
        
        public ISegment CurrentSegment {
            get { return _currentSegment; }
            set {
                _currentSegment = value;
                NotifyPropertyChanged("CurrentSegment");
            }
        }

        public ISegment RecordingSegment {
            get { return _recordingSegment; }
            set {
                _recordingSegment = value;
                NotifyPropertyChanged("RecordingSegment");
            }
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            //Todo: instantiate a new Segment
            ISegment newSegment = null;
            loadSplitView.DataContext = new EditSplitsViewModel(newSegment);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                CurrentSegment = newSegment;
                _currentGame = _splitsFacade.LoadGameManager(newSegment.ParentGameName);
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(CurrentSegment);
            loadSplitView.ShowDialog();

            /* Reload the current game if the parent game was edited */
            if (CurrentSegment.ParentGameName != _currentGame.GameName) {
                _currentGame = _splitsFacade.LoadGameManager(CurrentSegment.ParentGameName);
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                CurrentSegment = rsViewModel.SelectedSegment;
                _currentGame = _splitsFacade.LoadGameManager(CurrentSegment.ParentGameName);
            }
        }
    }
}
