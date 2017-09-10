using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSplitsViewModel
    {
        private ISplits _splits;

        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSegmentCommand { get; private set; }

        public EditSplitsViewModel(ISplits splits)
        {
            _splits = splits;
        }

        public IList<string> AvailableGames {
            get {
                throw new NotImplementedException();
            }
        }

        public string CurrentSplitName {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
