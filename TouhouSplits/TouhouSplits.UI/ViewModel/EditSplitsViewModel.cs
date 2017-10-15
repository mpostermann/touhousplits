using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSplitsViewModel : ViewModelBase, IDialogResultViewModel
    {
        private ISplitsFacade _splitsFacade;

        public IFileHandler<ISplits> SplitsFile { get; private set; }
        public ISplits Splits { get { return SplitsFile.Object; } }

        public event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
        private void InvokeRequestCloseDialog(RequestCloseDialogEventArgs e)
        {
            var handler = RequestCloseDialog;
            if (handler != null)
                handler(this, e);
        }

        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSplitsCommand { get; private set; }
        public ICommand SaveSplitsAsCommand { get; private set; }
        public ICommand CloseWithoutSavingCommand { get; private set; }

        public EditSplitsViewModel(IFileHandler<ISplits> splitsFile, ISplitsFacade facade)
        {
            SplitsFile = splitsFile;
            _splitsFacade = facade;
            if (_splitsFacade.AvailableGames.Contains(splitsFile.Object.GameId)) {
                _gameName = _splitsFacade.LoadGameManager(splitsFile.Object.GameId).GameName;
            };

            AddSegmentCommand = new RelayCommand<int>((param) => AddSegment(param));
            RemoveSegmentCommand = new RelayCommand<int>((param) => RemoveSegment(param));
            SaveSplitsCommand = new RelayCommand(() => SaveSplits());
            SaveSplitsAsCommand = new RelayCommand(() => SaveSplitsAs());
            CloseWithoutSavingCommand = new RelayCommand(() => CloseWithoutSaving());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGameNames;
            }
        }

        private string _gameName;
        public string GameName {
            get { return _gameName; }
            set {
                _gameName = value;
                Splits.GameId = _splitsFacade.GetIdFromName(_gameName);
            }
        }

        private void AddSegment(int index)
        {
            /* Check that index is within bounds */
            if (index < 0) {
                index = 0;
            }
            else if (index > Splits.Segments.Count) {
                index = Splits.Segments.Count;
            }

            Splits.AddSegment(index, new Segment());
        }

        private void RemoveSegment(int index)
        {
            if (index < 0)
                return;

            Splits.RemoveSegment(index);
        }

        private void SaveSplitsAs()
        {
            string validationMessage;
            if (!IsSplitsValid(SplitsFile.Object, out validationMessage)) {
                ShowErrorDialog(validationMessage);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            if (SplitsFile.FileInfo != null) {
                dialog.FileName = SplitsFile.FileInfo.FullName;
            }
            dialog.DefaultExt = FilePaths.EXT_SPLITS_FILE;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", FilePaths.EXT_SPLITS_FILE);

            if (dialog.ShowDialog() == true) {
                SplitsFile.FileInfo = new FileInfo(dialog.FileName);

                try {
                    SplitsFile.Save();
                    InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
                }
                catch (Exception e) {
                    ShowErrorDialog("File could not be saved. Please choose a different location and try again.");
                }
            }
        }

        private void SaveSplits()
        {
            if (SplitsFile.FileInfo != null) {
                SplitsFile.Save();
                InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
            }
            else {
                SaveSplitsAs();
            }
        }

        private static bool IsSplitsValid(ISplits splits, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(splits.GameId.ToString())) {
                message = "Select a game.";
                return false;
            }
            if (string.IsNullOrEmpty(splits.SplitName)) {
                message = "Enter a name for this splits.";
                return false;
            }
            if (splits.Segments.Count == 0) {
                message = "Add a segment.";
                return false;
            }
            foreach (ISegment segment in splits.Segments) {
                if (string.IsNullOrEmpty(segment.SegmentName)) {
                    message = "All segments must have a name.";
                    return false;
                }
            }
            return true;
        }

        private void CloseWithoutSaving()
        {
            SplitsFile.RevertToLastSavedState();
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false));
        }
    }
}
