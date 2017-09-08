﻿using System;
using System.Windows.Input;

namespace TouhouSplits.UI.ViweModel
{
    public class NewSplitsViewModel
    {
        public ICommand AddSegmentCommand { get; private set; }
        public ICommand UpdateSegmentNameCommand { get; private set; }
        public ICommand UpdateSegmentScoreCommand { get; private set; }
        public ICommand RemoveSegmentCommand { get; private set; }
        public ICommand SaveSegmentCommand { get; private set; }

        public NewSplitsViewModel()
        {
        }

        public string CurrentGame {
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
