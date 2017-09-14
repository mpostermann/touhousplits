using System.IO;

namespace TouhouSplits.Service.Data
{
    public interface ISplitsFile
    {
        FileInfo FileInfo { get; }
        ISplits Splits { get; }
    }
}
