using System.Collections.Generic;

namespace TouhouSplits.Service.Data
{
    public interface ISplits
    {
        string GameName { get; set; }
        string SplitName { get; set; }
        IList<ISegment> Segments { get; }
        ISegment EndingSegment { get; }
        void AddSegment(int index, ISegment segment);
        void RemoveSegment(int index);
        void UpdateSegment(int index, ISegment segment);
        ISplits Clone();
    }
}
