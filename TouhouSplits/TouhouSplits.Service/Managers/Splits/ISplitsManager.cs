using System.Collections.Generic;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Managers.Splits
{
    public interface ISplitsManager
    {
        IList<ISplits> RecentSplits { get; }
        ISplits DeserializeSplits(string filePath);
        void SerializeSplits(string filePath);
    }
}
