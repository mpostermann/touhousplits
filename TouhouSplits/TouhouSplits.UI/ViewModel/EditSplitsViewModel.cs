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

        public ISplits Splits { get; private set; }

        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSplitsCommand { get; private set; }

        public EditSplitsViewModel(ISplits splits, SplitsFacade facade)
        {
            //Splits = splits.Clone();
            Splits = splits;
            SplitsFacade = facade;

            AddSegmentCommand = new RelayCommand<int>((param) => AddSegment(param));
        }

        public IList<string> AvailableGames {
            get {
                return SplitsFacade.AvailableGames;
            }
        }

        public string GameName {
            get { return Splits.GameName; }
            set {
                Splits.GameName = value;
                NotifyPropertyChanged("GameName");
            }
        }

        public string SplitName {
            get { return Splits.SplitName; }
            set {
                Splits.SplitName = value;
                NotifyPropertyChanged("SplitName");
            }
        }

        public string SplitsFilePath {
            get {
                throw new NotImplementedException();
            }
        }

        private void AddSegment(int index) {
            /* Check that index is within bounds */
            if (index < 0) {
                index = 0;
            }
            else if (index > Splits.Segments.Count) {
                index = Splits.Segments.Count;
            }

            //Todo: Construct a new segment
            ISegment newSegment = null;

            Splits.AddSegment(index, newSegment);
        }
    }
}
