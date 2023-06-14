using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using TouhouSplits.Manager.Config;
using TouhouSplits.UI.Model;

namespace TouhouSplits.UI.ViewModel
{
    public class EditSettingsViewModel : ViewModelBase, IDialogResultViewModel
    {
        public event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
        private void InvokeRequestCloseDialog(RequestCloseDialogEventArgs e)
        {
            RequestCloseDialog?.Invoke(this, e);
        }

        public HotkeysConfigModel HotkeysConfig { get; }

        public ICommand RemoveHotkeyCommand { get; }
        public ICommand AddEmptyHotkeyCommand { get; }
        public ICommand EditHotkeyCommand { get; }
        public ICommand SaveAndCloseCommand { get; }
        public ICommand CancelAndCloseCommand { get; }

        public EditSettingsViewModel(IConfigManager configManager)
        {
            HotkeysConfig = new HotkeysConfigModel(configManager);

            RemoveHotkeyCommand = new RelayCommand<Keys>(keys => RemoveHotkey(keys));
            AddEmptyHotkeyCommand = new RelayCommand<Keys>(keys => AddEmptyHotkey(keys));
            EditHotkeyCommand = new RelayCommand<EditHotkeyCommandParameters>(param => EditHotkey(param.OriginalKeys, param.NewKeys));
            SaveAndCloseCommand = new RelayCommand(() => SaveAndClose());
            CancelAndCloseCommand = new RelayCommand(() => CancelAndClose());
        }

        private void RemoveHotkey(Keys keys)
        {
            HotkeysConfig.RemoveHotkey(keys);
        }

        private void AddEmptyHotkey(Keys keys)
        {
            HotkeysConfig.AddEmptyHotkey(keys);
        }

        private void EditHotkey(Keys originalKeys, Keys newKeys)
        {
            if (HotkeysConfig.HasHotkey(newKeys)) {
                var methodToRemove = HotkeysConfig.GetHotkeyOrNull(newKeys);
                var methodToAssign = HotkeysConfig.GetHotkeyOrNull(originalKeys);
                if (methodToRemove == methodToAssign) {
                    return;
                }

                var shouldOverwrite = ShowYesNoDialog($"{newKeys} is already bound to the action \"{methodToRemove.MethodName}\". Reassign it to \"{methodToAssign.MethodName}\"?", "Confirm Hotkey");
                if (shouldOverwrite != MessageBoxResult.Yes) {
                    return;
                }
                HotkeysConfig.RemoveHotkey(newKeys);
            }

            HotkeysConfig.EditHotkey(originalKeys, newKeys);
        }

        private void SaveAndClose()
        {
            HotkeysConfig.PersistChanges();
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true));
        }

        private void CancelAndClose()
        {
            InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false));
        }
    }
}
