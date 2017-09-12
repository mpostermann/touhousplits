using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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
        private Timer _recordTimer;

        private string _currentSplitsFilepath;
        private bool _isRecording;

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand RecentSplitsCommand { get; private set; }
        public ICommand NextSplitsCommand { get; private set; }
        public ICommand PreviousSplitsCommand { get; private set; }
        public ICommand StartOrStopRecordingSplitsCommand { get; private set; }

        public MainViewModel()
        {
            _splitsFacade = new SplitsFacade();
            _isRecording = false;

            NewSplitCommand = new RelayCommand(() => NewSplit());
            EditSplitCommand = new RelayCommand(() => EditSplit());
            RecentSplitsCommand = new RelayCommand(() => RecentSplits());
            NextSplitsCommand = new RelayCommand(() => NextSplits());
            PreviousSplitsCommand = new RelayCommand(() => PreviousSplits());
            StartOrStopRecordingSplitsCommand = new RelayCommand(() => StartOrStopRecordingSplits());
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

        public long CurrentScore {
            get { return _currentGame.Hook.GetCurrentScore(); }
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            //Todo: instantiate a new Segment
            ISplits newSplit = null;
            loadSplitView.DataContext = new EditSplitsViewModel(newSplit, _splitsFacade);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                _currentSplitsFilepath = loadSplitsVm.SplitsFilePath;
                CurrentSplits = loadSplitsVm.Splits;
                _currentGame = _splitsFacade.LoadGameManager(CurrentSplits.GameName);
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(CurrentSplits, _splitsFacade);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                _currentSplitsFilepath = loadSplitsVm.SplitsFilePath;
                CurrentSplits = loadSplitsVm.Splits;
                _currentGame = _splitsFacade.LoadGameManager(CurrentSplits.GameName);
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                _currentSplitsFilepath = rsViewModel.SplitsFilePath;
                CurrentSplits = rsViewModel.SelectedSplits;
                _currentGame = _splitsFacade.LoadGameManager(CurrentSplits.GameName);
            }
        }

        private void NextSplits()
        {
            if (CurrentSplits == null) {
                return;
            }

            /* Get the current index */
            int index = _currentGame.SplitsManager.RecentSplits.IndexOf(CurrentSplits);

            /* Get the next index */
            int nextIndex = index + 1;
            if (nextIndex == _currentGame.SplitsManager.RecentSplits.Count) {
                nextIndex = 0;
            }

            CurrentSplits = _currentGame.SplitsManager.RecentSplits[nextIndex];
        }

        private void PreviousSplits()
        {
            if (CurrentSplits == null) {
                return;
            }

            /* Get the current index */
            int index = _currentGame.SplitsManager.RecentSplits.IndexOf(CurrentSplits);

            /* Get the next index */
            int nextIndex = index - 1;
            if (nextIndex == -1) {
                nextIndex = _currentGame.SplitsManager.RecentSplits.Count - 1;
            }

            CurrentSplits = _currentGame.SplitsManager.RecentSplits[nextIndex];
        }

        private void StartOrStopRecordingSplits()
        {
            if (_isRecording) {
                StopRecordingSplits();
            }
            else {
                StartRecordingSplits();
            }
        }

        private void StartRecordingSplits()
        {
            if (_currentGame == null || _currentGame.Hook == null) {
                return;
            }

            _currentGame.Hook.Hook();

            //todo: Initialize splits builder and assign. If there is already a recording splits and it's better
            //than the current splits, then swap them.
            RecordingSplits = null;

            //Set a poller to check the updated score
            _recordTimer = new Timer(
                (param) => NotifyPropertyChanged("CurrentScore"),
                null,
                0,
                50
            );

            _isRecording = true;
        }

        private void StopRecordingSplits()
        {
            /* Stop the polling timer */
            _recordTimer.Dispose();
            _recordTimer = null;

            _currentGame.Hook.Unhook();

            /* If the new score is better than the previous, then save it */
            if (RecordingSplits.EndingSegment.Score > CurrentSplits.EndingSegment.Score) {
                _currentGame.SplitsManager.SerializeSplits(RecordingSplits, _currentSplitsFilepath);
            }

            _isRecording = false;
        }
    }
}
