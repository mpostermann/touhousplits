using System.Collections.Generic;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Managers.Splits
{
    public interface ISplitsManager
    {
        IList<ISplitsFile> RecentSplits { get; }
        ISplitsFile DeserializeSplits(string filePath);
        ISplitsFile SerializeSplits(ISplits splits, string filePath);
    }
}
