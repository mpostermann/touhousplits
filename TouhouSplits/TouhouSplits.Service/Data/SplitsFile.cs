using System.IO;

namespace TouhouSplits.Service.Data
{
    public class SplitsFile : ISplitsFile
    {
        public FileInfo FileInfo { get; private set; }
        public ISplits Splits { get; private set; }

        public SplitsFile(string filepath, ISplits splits)
        {
            FileInfo = new FileInfo(filepath);
            Splits = splits;
        }
    }
}
