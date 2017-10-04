using System;
using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.SplitsBuilder;

namespace TouhouSplits.Service.Managers
{
    public class PersonalBestSplitsBuilder : ISplitsBuilder
    {
        public PersonalBestSplitsBuilder(ISplits personalBest)
        {
            throw new NotImplementedException();
        }

        public IList<IPersonalBestSegment> Segments {
            get {
                throw new NotImplementedException();
            }
        }

        public void SetScoreForCurrentSegment(long score)
        {
            throw new NotImplementedException();
        }

        public void SplitToNextSegment()
        {
            throw new NotImplementedException();
        }

        public bool IsNewPersonalBest()
        {
            throw new NotImplementedException();
        }

        public ISplits GetOutput()
        {
            throw new NotImplementedException();
        }
    }
}
