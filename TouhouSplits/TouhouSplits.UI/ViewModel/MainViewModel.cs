﻿using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers;
using TouhouSplits.Service.Managers.Config;
using TouhouSplits.Service.Serialization;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel
    {
        private ISplitsFacade _splitsFacade;
        private IFileHandler<ISplits> _currentSplitsFile;

        private PersonalBestTracker _mainModel;
        public PersonalBestTracker MainModel { get { return _mainModel; } }

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand OpenSplitCommand { get; private set; }
        public ICommand NextSplitsCommand { get; private set; }
        public ICommand PreviousSplitsCommand { get; private set; }
        public ICommand StartOrStopRecordingSplitsCommand { get; private set; }
        public ICommand SplitToNextSegmentCommand { get; private set; }

        public MainViewModel()
        {
            IConfigManager configuration = new ConfigManager();
            _splitsFacade = new SplitsFacade(configuration, new JsonSerializer<Splits>());
            _mainModel = new PersonalBestTracker(_splitsFacade);

            NewSplitCommand = new RelayCommand(() => NewSplit(), () => !MainModel.IsPolling);
            EditSplitCommand = new RelayCommand(() => EditSplit(), () => _currentSplitsFile != null && !MainModel.IsPolling);
            OpenSplitCommand = new RelayCommand(() => OpenSplit(), () => !MainModel.IsPolling);
            NextSplitsCommand = new RelayCommand(() => NextSplits(), () => !MainModel.IsPolling);
            PreviousSplitsCommand = new RelayCommand(() => PreviousSplits(), () => !MainModel.IsPolling);
            StartOrStopRecordingSplitsCommand = new RelayCommand(() => StartOrStopRecordingSplits(), () => _currentSplitsFile != null);
            SplitToNextSegmentCommand = new RelayCommand(() => SplitToNextSegment(), () => MainModel.IsPolling);
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(_splitsFacade.NewSplitsFile(), _splitsFacade);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                ReloadCurrentSplitsFile(loadSplitsVm.SplitsFile);
            }
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(_currentSplitsFile, _splitsFacade);
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                ReloadCurrentSplitsFile(loadSplitsVm.SplitsFile);
            }
        }

        private void OpenSplit()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (_currentSplitsFile != null &&
                !string.IsNullOrEmpty(_currentSplitsFile.FileInfo.FullName)) {
                dialog.FileName = _currentSplitsFile.FileInfo.FullName;
            }
            dialog.DefaultExt = FilePaths.EXT_SPLITS_FILE;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", FilePaths.EXT_SPLITS_FILE);

            if (dialog.ShowDialog() == true) {
                var file = _splitsFacade.LoadSplitsFile(new FileInfo(dialog.FileName));
                ReloadCurrentSplitsFile(file);
            }
        }

        private void NextSplits()
        {
            if (_currentSplitsFile == null) {
                return;
            }

            var favoriteSplits = MainModel.FavoriteSplits();

            /* Get the current index */
            int index = favoriteSplits.IndexOf(_currentSplitsFile);

            /* Get the next index */
            int nextIndex = index + 1;
            if (nextIndex == favoriteSplits.Count) {
                nextIndex = 0;
            }

            _currentSplitsFile = favoriteSplits[nextIndex];
        }

        private void PreviousSplits()
        {
            if (_currentSplitsFile == null) {
                return;
            }

            var favoriteSplits = MainModel.FavoriteSplits();

            /* Get the current index */
            int index = favoriteSplits.IndexOf(_currentSplitsFile);

            /* Get the next index */
            int nextIndex = index - 1;
            if (nextIndex == -1) {
                nextIndex = favoriteSplits.Count - 1;
            }

            _currentSplitsFile = favoriteSplits[nextIndex];
        }

        private void ReloadCurrentSplitsFile(IFileHandler<ISplits> splitsFile)
        {
            if (_currentSplitsFile != null) {
                _currentSplitsFile.Close();
            }
            _currentSplitsFile = splitsFile;

            var personalBestBuilder = new PersonalBestSplitsBuilder(_currentSplitsFile.Object);
            MainModel.LoadPersonalBest(
                _currentSplitsFile.Object.GameId,
                _currentSplitsFile.Object.SplitName,
                personalBestBuilder
            );
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
        }

        private void StopRecordingSplits()
        {
            MainModel.StopScorePoller();

            /* Todo: If the new score is better than the previous, then save it */
        }

        private void SplitToNextSegment()
        {
            MainModel.SplitToNextSegment();
        }
    }
}
