using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class RecentSplitsViewModel
    {
        public ICommand RemoveSplits { get; private set; }

        public RecentSplitsViewModel()
        {
        }

        public IList<string> AvailableGames {
            get {
                throw new NotImplementedException();
            }
        }

        public IList<string> RecentSplits {
            get {
                throw new NotImplementedException();
            }
        }

        public ISegment SelectedSegment {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
