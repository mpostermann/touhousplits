using System.IO;
using TouhouSplits.Service.Serialization;

namespace TouhouSplits.Service.Data
{
    public class SplitsFile : ISplitsFile
    {
        private IFileSerializer<ISplits> _serializer;
        
        public FileInfo FileInfo { get; private set; }
        public ISplits Splits {
            get {
                return _serializer.Deserialize(FileInfo.FullName);
            }
        }

        public SplitsFile(string filepath, IFileSerializer<ISplits> serializer)
        {
            FileInfo = new FileInfo(filepath);
            _serializer = serializer;
        }
    }
}
