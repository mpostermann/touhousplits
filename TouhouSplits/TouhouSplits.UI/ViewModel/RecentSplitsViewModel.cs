using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.ViewModel
{
    public class RecentSplitsViewModel : ViewModelBase, IDialogResultViewModel
    {
        private SplitsFacade _splitsFacade;

        public ISplitsFile SelectedSplits { get; private set; }

        public event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
        private void InvokeRequestCloseDialog(RequestCloseDialogEventArgs e)
        {
            var handler = RequestCloseDialog;
            if (handler != null)
                handler(this, e);
        }

        public ICommand RemoveSplitsCommand { get; private set; }
        public ICommand OpenSplitsCommand { get; private set; }
        public ICommand CancelOpeningSplitsCommand { get; private set; }
        
        public RecentSplitsViewModel(SplitsFacade facade, ISplitsFile splitsFile)
        {
            _splitsFacade = facade;
            if (splitsFile != null) {
                CurrentGame = splitsFile.Splits.GameName;
            }

            OpenSplitsCommand = new RelayCommand<ISplitsFile>((param) => OpenSplits(param));
            CancelOpeningSplitsCommand = new RelayCommand(() => CancelOpeningSplits());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
            }
        }

        private string _currentGame;
        public string CurrentGame {
            get {
                return _currentGame;
            }
            set {
                _currentGame = value;
                RecentSplits = _splitsFacade.LoadGameManager(_currentGame).SplitsManager.RecentSplits;
                NotifyPropertyChanged("CurrentGame");
            }
        }

        private IList<ISplitsFile> _recentSplits;
        public IList<ISplitsFile> RecentSplits {
            get {
                return _recentSplits; 
            }
            set {
                _recentSplits = value;
                NotifyPropertyChanged("RecentSplits");
            }
        }

        private void OpenSplits(ISplitsFile file)
        {
            SelectedSplits = file;
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
        }

        private void CancelOpeningSplits()
        {
            SelectedSplits = null;
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false));
        }
    }
}
