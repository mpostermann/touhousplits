using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViweModel
{
    public class MainViewModel
    {
        private SplitsFacade _splitsFacade;

        public ICommand LoadGameCommand { get; private set; }
        public ICommand HookCurrentGameCommand { get; private set; }
        public ICommand UnhookCurrentGameCommand { get; private set; }
        public ICommand LoadSplitCommand { get; private set; }
        public ICommand NextSplitCommand { get; private set; }
        public ICommand PreviousSplitCommand { get; private set; }
        public ICommand StartRecordingSplitCommand { get; private set; }
        public ICommand StopRecordingSplitCommand { get; private set; }

        public MainViewModel()
        {
            _splitsFacade = new SplitsFacade();
        }

        public IList<string> AvailableGames {
            get {
                throw new NotImplementedException();
            }
        }

        public string CurrentGame {
            get {
                throw new NotImplementedException();
            }
        }

        public string CurrentSplit {
            get {
                throw new NotImplementedException();
            }
        }

        public ISegment CurrentSegment {
            get {
                throw new NotImplementedException();
            }
        }

        public ISegment RecordingSegment {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
