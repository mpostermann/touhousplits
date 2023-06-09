﻿using System.IO;

namespace TouhouSplits.Service.Data
{
    public interface IFileHandler<TOne>
    {
        FileInfo FileInfo { get; set; }
        TOne Object { get; }
        void Save();
        void RevertToLastSavedState();
        void Close();
    }
}
