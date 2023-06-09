﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;

namespace TouhouSplits.UI.ViewModel
{
    public class FavoriteSplitsViewModel : IDialogResultViewModel
    {
        private ISplitsFacade _splitsFacade;

        public readonly FavoriteSplitsModel FavoriteSplitsModel;
        public IFileHandler<ISplits> SelectedSplits { get; private set; }

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
        
        public FavoriteSplitsViewModel(ISplitsFacade facade, string initialGameName = null)
        {
            _splitsFacade = facade;

            if (!string.IsNullOrEmpty(initialGameName)) {
                FavoriteSplitsModel = new FavoriteSplitsModel(_splitsFacade.LoadGameManager(initialGameName));
            }
            else {
                FavoriteSplitsModel = new FavoriteSplitsModel(_splitsFacade.LoadGameManager(AvailableGames[0]));
            }

            OpenSplitsCommand = new RelayCommand<IFileHandler<ISplits>>((param) => OpenSplits(param));
            CancelOpeningSplitsCommand = new RelayCommand(() => CancelOpeningSplits());
        }

        public IList<string> AvailableGames {
            get {
                return _splitsFacade.AvailableGames;
            }
        }

        private void OpenSplits(IFileHandler<ISplits> file)
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
