using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Config;
using TouhouSplits.Service.Serialization;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel
    {
        private ISplitsFacade _splitsFacade;

        private MainModel _mainModel;
        public MainModel MainModel { get { return _mainModel; } }

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand OpenSplitCommand { get; private set; }
        public ICommand RecentSplitsCommand { get; private set; }
        public ICommand NextSplitsCommand { get; private set; }
        public ICommand PreviousSplitsCommand { get; private set; }
        public ICommand StartOrStopRecordingSplitsCommand { get; private set; }

        public MainViewModel()
        {
            IConfigManager configuration = new ConfigManager();
            _splitsFacade = new SplitsFacade(configuration);
            _mainModel = new MainModel(_splitsFacade);

            NewSplitCommand = new RelayCommand(() => NewSplit());
            EditSplitCommand = new RelayCommand(() => EditSplit());
            OpenSplitCommand = new RelayCommand(() => OpenSplit());
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
                MainModel.CurrentSplitsFile = loadSplitsVm.SplitsFile;
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(
                MainModel.CurrentSplitsFile.FileInfo.FullName,
                MainModel.CurrentSplitsFile.Splits,
                _splitsFacade
            );
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                MainModel.CurrentSplitsFile = loadSplitsVm.SplitsFile;
            }
        }

        private void OpenSplit()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (MainModel.CurrentSplitsFile != null &&
                !string.IsNullOrEmpty(MainModel.CurrentSplitsFile.FileInfo.FullName)) {
                dialog.FileName = MainModel.CurrentSplitsFile.FileInfo.FullName;
            }
            dialog.DefaultExt = FilePaths.EXT_SPLITS_FILE;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", FilePaths.EXT_SPLITS_FILE);

            if (dialog.ShowDialog() == true) {
                throw new NotImplementedException();
            }
        }

        private void RecentSplits()
        {
            var recentSplitsView = new RecentSplitsWindow();
            recentSplitsView.DataContext = new RecentSplitsViewModel(_splitsFacade, MainModel.CurrentSplitsFile.Splits);
            recentSplitsView.ShowDialog();

            if (recentSplitsView.DialogResult == true) {
                var rsViewModel = (RecentSplitsViewModel)recentSplitsView.DataContext;
                MainModel.CurrentSplitsFile = rsViewModel.SelectedSplits;
            }
        }

        private void NextSplits()
        {
            if (MainModel.CurrentSplitsFile == null) {
                return;
            }

            /* Get the current index */
            int index = MainModel.RecentSplits.IndexOf(MainModel.CurrentSplitsFile);

            /* Get the next index */
            int nextIndex = index + 1;
            if (nextIndex == MainModel.RecentSplits.Count) {
                nextIndex = 0;
            }

            MainModel.CurrentSplitsFile = MainModel.RecentSplits[nextIndex];
        }

        private void PreviousSplits()
        {
            if (MainModel.CurrentSplitsFile == null) {
                return;
            }

            /* Get the current index */
            int index = MainModel.RecentSplits.IndexOf(MainModel.CurrentSplitsFile);

            /* Get the next index */
            int nextIndex = index - 1;
            if (nextIndex == -1) {
                nextIndex = MainModel.RecentSplits.Count - 1;
            }

            MainModel.CurrentSplitsFile = MainModel.RecentSplits[nextIndex];
        }

        private void StartOrStopRecordingSplits()
        {
            if (MainModel.IsPolling) {
                StopRecordingSplits();
            }
            else {
                StartRecordingSplits();
            }
        }

        private void StartRecordingSplits()
        {
            MainModel.StartScorePoller();

            //todo: Initialize splits builder and assign. If there is already a recording splits and it's better
            //than the current splits, then swap them.
            MainModel.RecordingSplits = null;
        }

        private void StopRecordingSplits()
        {
            MainModel.StopScorePoller();

            /* If the new score is better than the previous, then save it */
            if (MainModel.RecordingSplits.EndingSegment.Score > MainModel.CurrentSplitsFile.Splits.EndingSegment.Score) {
                var gameManager = _splitsFacade.LoadGameManager(MainModel.RecordingSplits.GameName);
                _splitsFacade.SerializeSplits(MainModel.RecordingSplits, MainModel.CurrentSplitsFile.FileInfo);
            }
        }
    }
}
