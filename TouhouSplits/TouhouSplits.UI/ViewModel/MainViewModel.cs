using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Config;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel
    {
        private ISplitsFacade _splitsFacade;

        public Game CurrentGame { get; private set; }
        public ISplitsFile CurrentSplitsFile { get; private set; }
        public ISplits RecordingSplits { get; private set; }

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand RecentSplitsCommand { get; private set; }
        public ICommand NextSplitsCommand { get; private set; }
        public ICommand PreviousSplitsCommand { get; private set; }
        public ICommand StartOrStopRecordingSplitsCommand { get; private set; }

        public MainViewModel()
        {
            IConfigManager configuration = new ConfigManager();
            _splitsFacade = new SplitsFacade(configuration);

            NewSplitCommand = new RelayCommand(() => NewSplit());
            EditSplitCommand = new RelayCommand(() => EditSplit());
            RecentSplitsCommand = new RelayCommand(() => RecentSplits());
            NextSplitsCommand = new RelayCommand(() => NextSplits());
            PreviousSplitsCommand = new RelayCommand(() => PreviousSplits());
            StartOrStopRecordingSplitsCommand = new RelayCommand(() => StartOrStopRecordingSplits());
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(
                string.Empty,
                new Splits(),
                _splitsFacade
            );
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                CurrentSplitsFile = loadSplitsVm.SplitsFile;
                CurrentGame = new Game (_splitsFacade, _splitsFacade.LoadGameManager(CurrentSplitsFile.Splits.GameName));
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(
                CurrentSplitsFile.FileInfo.FullName,
                CurrentSplitsFile.Splits,
                _splitsFacade
            );
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                CurrentSplitsFile = loadSplitsVm.SplitsFile;
                CurrentGame = new Game(_splitsFacade, _splitsFacade.LoadGameManager(CurrentSplitsFile.Splits.GameName));
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.DataContext = new RecentSplitsViewModel(_splitsFacade, CurrentGame);
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                CurrentSplitsFile = rsViewModel.SelectedSplits;
                CurrentGame = new Game(_splitsFacade, _splitsFacade.LoadGameManager(CurrentSplitsFile.Splits.GameName));
            }
        }

        private void NextSplits()
        {
            if (CurrentSplitsFile == null) {
                return;
            }

            /* Get the current index */
            int index = CurrentGame.RecentSplits.IndexOf(CurrentSplitsFile);

            /* Get the next index */
            int nextIndex = index + 1;
            if (nextIndex == CurrentGame.RecentSplits.Count) {
                nextIndex = 0;
            }

            CurrentSplitsFile = CurrentGame.RecentSplits[nextIndex];
        }

        private void PreviousSplits()
        {
            if (CurrentSplitsFile == null) {
                return;
            }

            /* Get the current index */
            int index = CurrentGame.RecentSplits.IndexOf(CurrentSplitsFile);

            /* Get the next index */
            int nextIndex = index - 1;
            if (nextIndex == -1) {
                nextIndex = CurrentGame.RecentSplits.Count - 1;
            }

            CurrentSplitsFile = CurrentGame.RecentSplits[nextIndex];
        }

        private void StartOrStopRecordingSplits()
        {
            if (CurrentGame.IsPolling) {
                StopRecordingSplits();
            }
            else {
                StartRecordingSplits();
            }
        }

        private void StartRecordingSplits()
        {
            CurrentGame.StartScorePoller();

            //todo: Initialize splits builder and assign. If there is already a recording splits and it's better
            //than the current splits, then swap them.
            RecordingSplits = null;
        }

        private void StopRecordingSplits()
        {
            CurrentGame.StopScorePoller();

            /* If the new score is better than the previous, then save it */
            if (RecordingSplits.EndingSegment.Score > CurrentSplitsFile.Splits.EndingSegment.Score) {
                CurrentGame.GameManager.SerializeSplits(RecordingSplits, CurrentSplitsFile.FileInfo.FullName);
            }
        }
    }
}
