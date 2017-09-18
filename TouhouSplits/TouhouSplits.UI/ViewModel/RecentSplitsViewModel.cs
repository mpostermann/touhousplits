using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;

namespace TouhouSplits.UI.ViewModel
{
    public class RecentSplitsViewModel : IDialogResultViewModel
    {
        private SplitsFacade _splitsFacade;
        private IGameManager _gameManager;

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
                _gameManager = _splitsFacade.LoadGameManager(splitsFile.Splits.GameName);
            }

            OpenSplitsCommand = new RelayCommand<ISplitsFile>((param) => OpenSplits(param));
            CancelOpeningSplitsCommand = new RelayCommand(() => CancelOpeningSplits());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
            }
        }

        public string CurrentGame {
            get {
                return _gameManager.GameName;
            }
            set {
                _gameManager = _splitsFacade.LoadGameManager(value);
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
