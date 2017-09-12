using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSplitsViewModel : ViewModelBase
    {
        private ISplits _splits;
        private SplitsFacade _splitsFacade; 
        
        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSplitsCommand { get; private set; }

        public EditSplitsViewModel(ISplits splits, SplitsFacade facade)
        {
            _splits = splits;
            _splitsFacade = facade;
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
            get {
                throw new NotImplementedException();
            }
        }
    }
}
