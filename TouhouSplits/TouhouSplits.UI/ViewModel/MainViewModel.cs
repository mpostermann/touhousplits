using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Configuration;
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
using TouhouSplits.UI.Dialog;
using TouhouSplits.UI.Hotkey;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.View;

namespace TouhouSplits.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ISplitsFacade _splitsFacade;
        private bool _dialogIsOpen;

        private PersonalBestTracker _mainModel;
        public PersonalBestTracker MainModel { get { return _mainModel; } }

        private SaveLoadHandler _fileModel;
        public SaveLoadHandler FileModel { get { return _fileModel; } }

        public ICommand NewSplitCommand { get; private set; }
        public ICommand EditSplitCommand { get; private set; }
        public ICommand OpenSplitCommand { get; private set; }
        public ICommand NextSplitsCommand { get; private set; }
        public ICommand PreviousSplitsCommand { get; private set; }
        public ICommand StartOrStopRecordingSplitsCommand { get; private set; }
        public ICommand SplitToNextSegmentCommand { get; private set; }
        public ICommand SaveCurrentSplitsAsCommand { get; private set; }
        public ICommand SavePollingErrorBugReportCommand { get; private set; }
        public ICommand ClearPollingErrorCommand { get; private set; }
        public ICommand ExitApplicationCommand { get; private set; }

        public MainViewModel()
        {
            Application.Current.DispatcherUnhandledException -= CrashDialog.UnhandledEventHandler;
            Application.Current.DispatcherUnhandledException += CrashDialog.UnhandledEventHandler;

            IConfigManager configuration = LoadConfiguration();
            _splitsFacade = new SplitsFacade(configuration, new JsonSerializer<Splits>());
            _mainModel = new PersonalBestTracker(_splitsFacade);
            _fileModel = new SaveLoadHandler(_splitsFacade);
            _dialogIsOpen = false;

            /* Register UI commands */
            NewSplitCommand = new RelayCommand(() => NewSplit(), () => !MainModel.IsPolling);
            EditSplitCommand = new RelayCommand(() => EditSplit(), () => FileModel.CurrentFile() != null && !MainModel.IsPolling);
            OpenSplitCommand = new RelayCommand(() => OpenSplit(), () => !MainModel.IsPolling);
            NextSplitsCommand = new RelayCommand(() => NextSplits(), () => !MainModel.IsPolling);
            PreviousSplitsCommand = new RelayCommand(() => PreviousSplits(), () => !MainModel.IsPolling);
            StartOrStopRecordingSplitsCommand = new RelayCommand(() => StartOrStopRecordingSplits(), () => FileModel.CurrentFile() != null && !_dialogIsOpen);
            SplitToNextSegmentCommand = new RelayCommand(() => SplitToNextSegment(), () => MainModel.IsPolling && !_dialogIsOpen);
            SaveCurrentSplitsAsCommand = new RelayCommand(() => SaveCurrentSplitsAs(), () => FileModel.CurrentFile() != null && !MainModel.IsPolling);
            SavePollingErrorBugReportCommand = new RelayCommand(() => SavePollingErrorBugReport(), () => !MainModel.IsPolling && MainModel.HasError);
            ClearPollingErrorCommand = new RelayCommand(() => ClearPollingError(), () => !MainModel.IsPolling && MainModel.HasError);
            ExitApplicationCommand = new RelayCommand(() => ExitApplication());

            RegisterHotkeys(configuration.Hotkeys);
        }

        private static IConfigManager LoadConfiguration()
        {
            try {
                return new ConfigManager();
            }
            catch (ConfigurationErrorsException e) {
                ShowErrorDialog(e.Message);
                Application.Current.Shutdown();
                return null;
            }
        }

        private void RegisterHotkeys(IHotkeyConfig config)
        {
            if (config.HasHotkey("ToggleHotkeys")) {
                foreach (System.Windows.Forms.Keys keys in config.GetHotkeys("ToggleHotkeys")) {
                    GlobalHotkeyManagerFactory.Instance.RegisterEnableToggleHotkey(keys);
                }
            }
            RegisterSingleHotkey(config, "StartOrStopRecordingSplits", StartOrStopRecordingSplitsCommand);
            RegisterSingleHotkey(config, "SplitToNextSegment", SplitToNextSegmentCommand);
        }

        private static void RegisterSingleHotkey(IHotkeyConfig config, string hotkeyName, ICommand command)
        {
            if (config.HasHotkey(hotkeyName)) {
                foreach (System.Windows.Forms.Keys keys in config.GetHotkeys(hotkeyName)) {
                    GlobalHotkeyManagerFactory.Instance.RegisterHotkey(keys, command);
                }
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
            loadSplitView.DataContext = new EditSplitsViewModel(FileModel.CurrentFile(), _splitsFacade);
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
            if (!string.IsNullOrEmpty(FileModel.FileName)) {
                dialog.FileName = FileModel.FileName;
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
            if (FileModel.CurrentFile() == null) {
                return;
            }

            var favoriteSplits = MainModel.FavoriteSplits();

            /* Get the current index */
            int index = favoriteSplits.IndexOf(FileModel.CurrentFile());

            /* Get the next index */
            int nextIndex = index + 1;
            if (nextIndex == favoriteSplits.Count) {
                nextIndex = 0;
            }

            FileModel.LoadSplitsFile(favoriteSplits[nextIndex]);
        }

        private void PreviousSplits()
        {
            if (FileModel.CurrentFile() == null) {
                return;
            }

            var favoriteSplits = MainModel.FavoriteSplits();

            /* Get the current index */
            int index = favoriteSplits.IndexOf(FileModel.CurrentFile());

            /* Get the next index */
            int nextIndex = index - 1;
            if (nextIndex == -1) {
                nextIndex = favoriteSplits.Count - 1;
            }

            FileModel.LoadSplitsFile(favoriteSplits[nextIndex]);
        }

        private void ReloadCurrentSplitsFile(IFileHandler<ISplits> splitsFile)
        {
            try {
                ClearPollingError();
                FileModel.LoadSplitsFile(splitsFile);

                var personalBestBuilder = new PersonalBestSplitsBuilder(splitsFile.Object);
                MainModel.LoadPersonalBest(
                    splitsFile.Object.GameId,
                    splitsFile.Object.SplitName,
                    personalBestBuilder
                );
            }
            catch (Exception e) {
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
            ClearPollingError();
            if (MainModel.IsNewPersonalBest) {
                ReloadCurrentSplitsFile(FileModel.CurrentFile());
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
                /* Save the new splits, but don't reload it to the MainModel yet
                 * so the player can see how much better their segments were. */
                string errorMessage;
                FileModel.SaveToCurrentFile(newSplits, out errorMessage);
            }
        }

        private void SaveCurrentSplitsAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (!string.IsNullOrEmpty(FileModel.FileName)) {
                dialog.FileName = FileModel.FileName;
            }
            dialog.DefaultExt = FilePaths.EXT_SPLITS_FILE;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", FilePaths.EXT_SPLITS_FILE);

            if (dialog.ShowDialog() == true) {
                var fileInfo = new FileInfo(dialog.FileName);
                string errorMessage;
                FileModel.SaveCurrentSplitsAs(fileInfo, out errorMessage);
            }
        }

        private void SavePollingErrorBugReport()
        {
            var bugReporter = new BugReporter();
            string error;
            if (bugReporter.ShowSaveBugReportDialog(MainModel.LastError, out error)) {
                ClearPollingError();
            }
            else {
                ShowErrorDialog("Unable to save bug report to the specified location. Please select a different file location and try again.\n\n Error: " + error);
            }
        }

        private void ClearPollingError()
        {
            MainModel.ClearError();
        }

        private void ExitApplication()
        {
            if (MainModel.IsPolling) {
                var result = MessageBox.Show("Score polling is in progress. Are you sure you want to exit?", "Polling In Progress", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) {
                    return;
                }
            }

            Application.Current.Shutdown();
        }
    }       
}
