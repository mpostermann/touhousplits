﻿using System.Collections.Generic;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Managers.SplitsBuilder
{
    public interface ISplitsBuilder
    {
        IList<IPersonalBestSegment> Segments { get; }
        int CurrentSegment { get; }
        void SetScoreForCurrentSegment(long score);
        void SplitToNextSegment();
        void Reset();
        void MarkAsStarted();
        void MarkAsStopped();
        bool IsNewPersonalBest();
        ISplits GetOutput();
    }
}
