using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSplitsViewModel : ViewModelBase
    {
        private SplitsFacade SplitsFacade;
        private ISplits _splits;
        private string _filepath;

        public ISplitsFile SplitsFile { get; private set; }

        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSplitsCommand { get; private set; }

        public EditSplitsViewModel(ISplitsFile splits, SplitsFacade facade)
        {
            _splits = splits.Splits.Clone();
            _filepath = splits.FileInfo.FullName;

            SplitsFacade = facade;

            AddSegmentCommand = new RelayCommand<int>((param) => AddSegment(param));
        }

        public IList<string> AvailableGames {
            get {
                return SplitsFacade.AvailableGames;
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
    }
}
