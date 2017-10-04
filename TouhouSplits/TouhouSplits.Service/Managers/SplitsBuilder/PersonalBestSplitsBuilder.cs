using System;
using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.SplitsBuilder;
using TouhouSplits.UI.Model;

namespace TouhouSplits.Service.Managers
{
    public class PersonalBestSplitsBuilder : ISplitsBuilder
    {
        private int _currentSegment;

        public PersonalBestSplitsBuilder(ISplits personalBest)
        {
            _currentSegment = 0;

            _segments = new List<IPersonalBestSegment>();
            foreach (ISegment segment in personalBest.Segments) {
                var pbSegment = new PersonalBestSegment(segment);
                _segments.Add(pbSegment);
            }
        }

        private List<IPersonalBestSegment> _segments;
        public IList<IPersonalBestSegment> Segments {
            get { return _segments.AsReadOnly(); }
        }

        public void SetScoreForCurrentSegment(long score)
        {
            var currentSegment = ((PersonalBestSegment)_segments[_currentSegment]);
            currentSegment.RecordingScore = score;
        }

        public void SplitToNextSegment()
        {
            if (_currentSegment == _segments.Count) {
                throw new InvalidOperationException("Cannot split past the last existing segment.");
            }
            _currentSegment++;
        }

        public bool IsNewPersonalBest()
        {
            long currentScore = _segments[_currentSegment].RecordingScore;
            long personalBest = _segments[_segments.Count - 1].PersonalBestScore;
            return currentScore > personalBest;
        }

        public ISplits GetOutput()
        {
            var newSplits = new Splits();
            long previousScore = 0;
            for (int i = 0; i < _segments.Count; i++) { 
                var newSegment = new Segment() {
                    SegmentName = _segments[i].SegmentName,
                    Score = _segments[i].RecordingScore
                };
                if (newSegment.Score < previousScore) {
                    newSegment.Score = previousScore;
                }

                newSplits.AddSegment(i, newSegment);
                previousScore = newSegment.Score;
            }

            return newSplits;
        }
    }
}
