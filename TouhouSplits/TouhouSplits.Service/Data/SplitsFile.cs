using System.IO;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service.Data
{
    public class SplitsFile : ISplitsFile
    {
        private IFileSerializer<ISplits> _serializer;
        
        public FileInfo FileInfo { get; private set; }

        private ISplits _splits;
        public ISplits Splits {
            get {
                if (_splits == null) {
                    _splits = _serializer.Deserialize(FileInfo.FullName);
                }
                return _splits;
            }
        }

        public SplitsFile(string filepath, IFileSerializer<ISplits> serializer)
        {
            FileInfo = new FileInfo(filepath);
            _serializer = serializer;
        }

        public SplitsFile(string filepath, ISplits splits)
        {
            FileInfo = new FileInfo(filepath);
            _splits = splits;
        }
    }
}
