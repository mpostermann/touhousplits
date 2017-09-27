using System.IO;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service.Data
{
    public class SplitsFile : ISplitsFile
    {
        private IFileSerializer<Splits> _serializer;
        
        public FileInfo FileInfo { get; private set; }

        private ISplits _splits;
        public ISplits Splits {
            get {
                if (_splits == null) {
                    _splits = _serializer.Deserialize(FileInfo);
                }
                return _splits;
            }
        }

        public SplitsFile(FileInfo fileInfo, IFileSerializer<Splits> serializer)
        {
            FileInfo = fileInfo;
            _serializer = serializer;
        }

        public SplitsFile(FileInfo fileInfo, ISplits splits)
        {
            FileInfo = fileInfo;
            _splits = splits;
        }
    }
}
