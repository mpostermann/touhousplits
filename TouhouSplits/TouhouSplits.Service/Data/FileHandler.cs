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
        private TTwo _object;
        private TTwo _clone;
        private bool _isLoaded;

        public FileHandler(FileInfo fileInfo, IFileSerializer<TTwo> serializer)
        {
            FileInfo = fileInfo;
            _serializer = serializer;
            _isLoaded = false;
        }

        public FileHandler(TTwo obj, IFileSerializer<TTwo> serializer)
        {
            if (obj == null) {
                throw new NullReferenceException("FileHandler requires a non-null obj parameter.");
            }

            _object = obj;
            _clone = (TTwo) obj.Clone();
            _serializer = serializer;
            _isLoaded = true;
        }

        public FileInfo FileInfo { get; set; }

        public TOne Object {
            get {
                if (!_isLoaded) {
                    if (FileInfo == null) {
                        throw new InvalidOperationException("Object cannot be loaded because no FileInfo is set.");
                    }
                    _object = _serializer.Deserialize(FileInfo);
                    _clone = (TTwo) _object.Clone();
                    _isLoaded = true;
                }
                return _object;
            }
        }

        public void Close()
        {
            _object = default(TTwo);
            _isLoaded = false;
        }

        public void RevertToLastSavedState()
        {
            _object = _clone;
            _clone = (TTwo) _object.Clone();
        }

        public void Save()
        {
            if (FileInfo == null) {
                throw new InvalidOperationException("Object cannot be saved because no FileInfo is set.");
            }
            _serializer.Serialize((TTwo) Object, FileInfo);
            _clone = (TTwo) _object.Clone();
        }
    }
}
