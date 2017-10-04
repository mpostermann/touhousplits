using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;

namespace TouhouSplits.Service.Managers.SplitsBuilder
{
    public interface ISplitsBuilder
    {
        IList<IPersonalBestSegment> Segments { get; }
        void SetScoreForCurrentSegment(long score);
        void SplitToNextSegment();
        bool IsNewPersonalBest();
        ISplits GetOutput();
    }
}
