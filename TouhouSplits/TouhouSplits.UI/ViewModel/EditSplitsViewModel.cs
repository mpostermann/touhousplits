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
    public class EditSplitsViewModel : IDialogResultViewModel
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

            AddSegmentCommand = new RelayCommand<int>((param) => AddSegment(param));
            RemoveSegmentCommand = new RelayCommand<int>((param) => RemoveSegment(param));
            SaveSplitsCommand = new RelayCommand(() => SaveSplits());
            SaveSplitsAsCommand = new RelayCommand(() => SaveSplitsAs());
            CloseWithoutSavingCommand = new RelayCommand(() => CloseWithoutSaving());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
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
            SaveFileDialog dialog = new SaveFileDialog();
            if (SplitsFile.FileInfo != null) {
                dialog.FileName = SplitsFile.FileInfo.FullName;
            }
            dialog.DefaultExt = FilePaths.EXT_SPLITS_FILE;
            dialog.Filter = string.Format("Touhou Splits Files ({0})|*{0}", FilePaths.EXT_SPLITS_FILE);

            if (dialog.ShowDialog() == true) {
                SplitsFile.Save(new FileInfo(dialog.FileName));
                InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
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

        private void CloseWithoutSaving()
        {
            SplitsFile.RevertToLastSavedState();
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false));
        }
    }
}
