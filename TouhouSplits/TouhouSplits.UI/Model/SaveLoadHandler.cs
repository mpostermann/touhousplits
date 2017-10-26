using System;
using System.IO;
using TouhouSplits.MVVM;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;

namespace TouhouSplits.UI.Model
{
    public class SaveLoadHandler : ModelBase
    {
        private ISplitsFacade _splitsFacade;
        private IFileHandler<ISplits> _currentSplitsFile;
        private bool _hasUnsavedChanges;

        public SaveLoadHandler(ISplitsFacade splitsFacade)
        {
            _splitsFacade = splitsFacade;
            HasUnsavedChanges = false;
        }

        public string FileName {
            get {
                if (_currentSplitsFile != null && _currentSplitsFile.FileInfo != null) {
                    return _currentSplitsFile.FileInfo.Name;
                }
                return string.Empty;
            }
        }

        public bool HasUnsavedChanges {
            get { return _hasUnsavedChanges; }
            private set {
                _hasUnsavedChanges = value;
                NotifyPropertyChanged("HasUnsavedChanges");
            }
        }

        /// <summary>
        /// Returns the currently loaded splits file. Returns null if no file is loaded.
        /// </summary>
        public IFileHandler<ISplits> CurrentFile()
        {
            return _currentSplitsFile;
        }

        public void LoadSplitsFile(IFileHandler<ISplits> splitsFile)
        {
            /* This is kinda hacky, but before doing anything else we reference the file handler's
             * Object property to force it to load the object from file so we know the file is loadable */
            var splits = splitsFile.Object;

            if (_currentSplitsFile != null) {
                _currentSplitsFile.Close();
            }
            _currentSplitsFile = splitsFile;
            HasUnsavedChanges = false;
            NotifyPropertyChanged("FileName");
        }

        public bool SaveToCurrentFile(ISplits newSplits, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_currentSplitsFile == null || _currentSplitsFile.FileInfo == null) {
                throw new InvalidOperationException("No FileInfo is set for the current file, or no file is currently loaded.");
            }

            var newSplitsFile = _splitsFacade.NewSplitsFile(newSplits);
            newSplitsFile.FileInfo = _currentSplitsFile.FileInfo;

            try {
                _currentSplitsFile.Close();
                _currentSplitsFile = newSplitsFile;
                _currentSplitsFile.Save();
                HasUnsavedChanges = false;
            }
            catch (Exception e) {
                _currentSplitsFile = newSplitsFile;
                HasUnsavedChanges = true;
                errorMessage = e.Message;
            }

            return !HasUnsavedChanges;
        }

        public bool SaveCurrentSplitsAs(FileInfo newPath, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_currentSplitsFile == null) {
                throw new InvalidOperationException("No file is currently loaded.");
            }

            FileInfo originalFileInfo = _currentSplitsFile.FileInfo;
            try {
                _currentSplitsFile.FileInfo = newPath;
                _currentSplitsFile.Save();
                HasUnsavedChanges = false;
                NotifyPropertyChanged("FileName");
                return true;
            }
            catch (Exception e) {
                _currentSplitsFile.FileInfo = originalFileInfo;
                errorMessage = e.Message;
                return false;
            }
        }
    }
}
