using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers;
using TouhouSplits.Service.Managers.Config;
using TouhouSplits.Service.Serialization;
using TouhouSplits.UI.Hotkey;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ISplitsFacade _splitsFacade;
        private IFileHandler<ISplits> _currentSplitsFile;
        private bool _dialogIsOpen;

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
            _dialogIsOpen = false;

            /* Register UI commands */
            NewSplitCommand = new RelayCommand(() => NewSplit(), () => !MainModel.IsPolling);
            EditSplitCommand = new RelayCommand(() => EditSplit(), () => _currentSplitsFile != null && !MainModel.IsPolling);
            OpenSplitCommand = new RelayCommand(() => OpenSplit(), () => !MainModel.IsPolling);
            NextSplitsCommand = new RelayCommand(() => NextSplits(), () => !MainModel.IsPolling);
            PreviousSplitsCommand = new RelayCommand(() => PreviousSplits(), () => !MainModel.IsPolling);
            StartOrStopRecordingSplitsCommand = new RelayCommand(() => StartOrStopRecordingSplits(), () => _currentSplitsFile != null && !_dialogIsOpen);
            SplitToNextSegmentCommand = new RelayCommand(() => SplitToNextSegment(), () => MainModel.IsPolling && !_dialogIsOpen);

            RegisterHotkeys(configuration.Hotkeys);
        }

        private void RegisterHotkeys(IHotkeyConfig config)
        {
            if (config.HasHotkey("ToggleHotkeys")) {
                GlobalHotkeyManagerFactory.Instance.RegisterEnableToggleHotkey(config.GetHotkey("ToggleHotkeys"));
            }
            RegisterSingleHotkey(config, "StartOrStopRecordingSplits", StartOrStopRecordingSplitsCommand);
            RegisterSingleHotkey(config, "SplitToNextSegment", SplitToNextSegmentCommand);
        }

        private static void RegisterSingleHotkey(IHotkeyConfig config, string hotkeyName, ICommand command)
        {
            if (config.HasHotkey(hotkeyName)) {
                GlobalHotkeyManagerFactory.Instance.RegisterHotkey(config.GetHotkey(hotkeyName), command);
            }
        }

        private void NewSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(_splitsFacade.NewSplitsFile(), _splitsFacade);
            _dialogIsOpen = true;
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                ReloadCurrentSplitsFile(loadSplitsVm.SplitsFile);
            }
            _dialogIsOpen = false;
        }

        private void EditSplit()
        {
            var loadSplitView = new EditSplitsWindow();
            loadSplitView.DataContext = new EditSplitsViewModel(_currentSplitsFile, _splitsFacade);
            _dialogIsOpen = true;
            loadSplitView.ShowDialog();

            if (loadSplitView.DialogResult == true) {
                var loadSplitsVm = (EditSplitsViewModel)loadSplitView.DataContext;
                ReloadCurrentSplitsFile(loadSplitsVm.SplitsFile);
            }
            _dialogIsOpen = false;
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

            _dialogIsOpen = true;
            if (dialog.ShowDialog() == true) {
                var file = _splitsFacade.LoadSplitsFile(new FileInfo(dialog.FileName));
                ReloadCurrentSplitsFile(file);
            }
            _dialogIsOpen = false;
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
            var origSplitsFile = _currentSplitsFile;
            try {
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
            catch (Exception e) {
                _currentSplitsFile = origSplitsFile;
                if (e is FileNotFoundException || e is DirectoryNotFoundException) {
                    ShowErrorDialog(e.Message);
                }
                else if (e is SerializationException || e is NotSupportedException) {
                    ShowErrorDialog(string.Format(
                        "An error occurred while opening the file \"{0}\". Check that the file is formatted correctly and is not corrupted.",
                        splitsFile.FileInfo.FullName));
                }
                else {
                    throw;
                }
            }
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
            if (MainModel.IsNewPersonalBest) {
                ReloadCurrentSplitsFile(_currentSplitsFile);
            }

            try {
                MainModel.StartScorePoller();
            }
            catch (InvalidOperationException e) {
                ShowErrorDialog(e.Message);
            }
        }

        private void StopRecordingSplits()
        {
            var newSplits = MainModel.StopScorePoller();
            UpdateSplitsFileIfNewPersonalBest(newSplits);
        }

        private void SplitToNextSegment()
        {
            MainModel.SplitToNextSegment();
            if (!MainModel.IsPolling) {
                StopRecordingSplits();
            }
        }

        private void UpdateSplitsFileIfNewPersonalBest(ISplits newSplits)
        {
            if (MainModel.IsNewPersonalBest) {
                var newSplitsFile = _splitsFacade.NewSplitsFile(newSplits);
                newSplitsFile.FileInfo = _currentSplitsFile.FileInfo;

                /* Save the new splits, but don't reload it to the MainModel yet
                 * so the player can see how much better their segments were. */
                _currentSplitsFile.Close();
                _currentSplitsFile = newSplitsFile;
                _currentSplitsFile.Save();
            }
        }
    }       
}
