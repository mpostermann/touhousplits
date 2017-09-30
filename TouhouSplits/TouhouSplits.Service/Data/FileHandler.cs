using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service.Data
{
    public class FileHandler<TOne, TTwo> : IFileHandler<TOne>
        where TTwo : ICloneable, TOne
    {
        private IFileSerializer<TTwo> _serializer;
        private TTwo _clone;

        public FileHandler(FileInfo fileInfo, IFileSerializer<TTwo> serializer)
        {
            FileInfo = fileInfo;
            _serializer = serializer;
        }

        public FileHandler(TTwo obj, IFileSerializer<TTwo> serializer)
        {
            Object = obj;
            _clone = (TTwo) obj.Clone();
            _serializer = serializer;
        }

        public FileInfo FileInfo { get; set; }

        public TOne Object { get; private set; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void RevertToLastSavedState()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
