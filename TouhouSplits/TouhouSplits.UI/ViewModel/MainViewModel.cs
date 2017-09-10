using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel
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

        public string CurrentGame {
            get {
                if (_currentGame == null) {
                    return string.Empty;
                }
                return _currentGame.GameName;
            }
        }

        public string CurrentSplit {
            get {
                if (_currentSegment == null) {
                    return string.Empty;
                }
                return _currentSegment.SegmentName;
            }
        }

        public ISegment CurrentSegment { get { return _currentSegment; } }
        public ISegment RecordingSegment { get { return _recordingSegment; } }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            //Todo: instantiate a new Segment
            ISegment newSegment = null;
            loadSplitView.DataContext = new EditSplitsViewModel(newSegment);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                _currentSegment = newSegment;
                _currentGame = _splitsFacade.LoadGameManager(newSegment.ParentGameName);
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(_currentSegment);
            loadSplitView.ShowDialog();

            /* Reload the current game if the parent game was edited */
            if (_currentSegment.ParentGameName != _currentGame.GameName) {
                _currentGame = _splitsFacade.LoadGameManager(_currentSegment.ParentGameName);
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                _currentSegment = rsViewModel.SelectedSegment;
                _currentGame = _splitsFacade.LoadGameManager(_currentSegment.ParentGameName);
            }
        }
    }
}
