using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;

namespace TouhouSplits.Service.Managers.SplitsBuilder
{
    public interface ISplitsBuilder
    {
        IList<IPersonalBestSegment> Segments { get; }
        int CurrentSegment { get; }
        void SetScoreForCurrentSegment(long score);
        void SplitToNextSegment();
        void Reset();
        bool IsNewPersonalBest();
        ISplits GetOutput();
    }
}
