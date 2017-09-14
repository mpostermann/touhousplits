using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSplitsViewModel : ViewModelBase, IDialogResultViewModel
    {
        private SplitsFacade _splitsFacade;
        private ISplits _splits;
        private string _filepath;

        public ISplitsFile SplitsFile { get; private set; }

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

        public EditSplitsViewModel(ISplitsFile splits, SplitsFacade facade)
        {
            _splits = splits.Splits.Clone();
            _filepath = splits.FileInfo.FullName;

            _splitsFacade = facade;

            AddSegmentCommand = new RelayCommand<int>((param) => AddSegment(param));
            SaveSplitsCommand = new RelayCommand(() => SaveSplits());
            SaveSplitsAsCommand = new RelayCommand(() => SaveSplitsAs());
            CloseWithoutSavingCommand = new RelayCommand(() => CloseWithoutSaving());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
            }
        }

        public string GameName {
            get { return _splits.GameName; }
            set {
                _splits.GameName = value;
                NotifyPropertyChanged("GameName");
            }
        }

        public string SplitName {
            get { return _splits.SplitName; }
            set {
                _splits.SplitName = value;
                NotifyPropertyChanged("SplitName");
            }
        }

        public string SplitsFilePath {
            get { return _filepath; }
            set {
                _filepath = value;
                NotifyPropertyChanged("SplitsFilePath");
            }
        }

        private void AddSegment(int index) {
            /* Check that index is within bounds */
            if (index < 0) {
                index = 0;
            }
            else if (index > _splits.Segments.Count) {
                index = _splits.Segments.Count;
            }

            //Todo: Construct a new segment
            ISegment newSegment = null;

            _splits.AddSegment(index, newSegment);
        }

        private void SaveSplitsAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (!String.IsNullOrEmpty(_filepath)) {
                dialog.FileName = _filepath;
            }
            dialog.DefaultExt = SplitsFacade.DEFAULT_FILE_EXT;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", SplitsFacade.DEFAULT_FILE_EXT);

            if (dialog.ShowDialog() == true) {
                SaveSplits(dialog.FileName);
                InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
            }
        }

        private void SaveSplits()
        {
            SaveSplits(_filepath);
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
        }

        private void SaveSplits(string filepath)
        {
            var splitsManager = _splitsFacade.LoadGameManager(GameName).SplitsManager;
            SplitsFile = splitsManager.SerializeSplits(_splits, filepath);
            SplitsFilePath = SplitsFile.FileInfo.FullName;
            _splits = SplitsFile.Splits;
        }

        private void CloseWithoutSaving()
        {
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false));
        }
    }
}
