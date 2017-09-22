using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;

namespace TouhouSplits.UI.ViewModel
{
    public class RecentSplitsViewModel : IDialogResultViewModel
    {
        private ISplitsFacade _splitsFacade;

        public Game CurrentGame { get; set; }
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
        
        public RecentSplitsViewModel(ISplitsFacade facade, Game game)
        {
            _splitsFacade = facade;
            CurrentGame = game;

            if (CurrentGame == null) {
                CurrentGame = new Game(_splitsFacade, _splitsFacade.LoadGameManager(AvailableGames[0]));
            }

            OpenSplitsCommand = new RelayCommand<ISplitsFile>((param) => OpenSplits(param));
            CancelOpeningSplitsCommand = new RelayCommand(() => CancelOpeningSplits());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
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
