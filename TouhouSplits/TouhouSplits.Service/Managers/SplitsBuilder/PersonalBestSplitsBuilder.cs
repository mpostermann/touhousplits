using System;
using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.SplitsBuilder;
using TouhouSplits.UI.Model;

namespace TouhouSplits.Service.Managers
{
    public class PersonalBestSplitsBuilder : ISplitsBuilder
    {
        private long _personalBestScore;
        GameId _gameId;
        string _splitsName;

        public PersonalBestSplitsBuilder(ISplits personalBest)
        {
            _currentSegment = 0;
            _personalBestScore = personalBest.EndingSegment.Score;
            _gameId = personalBest.GameId;
            _splitsName = personalBest.SplitName;

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

        private int _currentSegment;
        public int CurrentSegment {
            get { return _currentSegment; }
        }

        public void SetScoreForCurrentSegment(long score)
        {
            var currentSegment = ((PersonalBestSegment)_segments[_currentSegment]);
            currentSegment.RecordingScore = score;
        }

        public void SplitToNextSegment()
        {
            if (_currentSegment == _segments.Count - 1) {
                throw new InvalidOperationException("Cannot split past the last existing segment.");
            }
            _currentSegment++;
        }

        public void Reset()
        {
            _currentSegment = 0;
            foreach (PersonalBestSegment segment in _segments) {
                segment.RecordingScore = PersonalBestSegment.UNSET_SCORE;
            }
        }

        public bool IsNewPersonalBest()
        {
            long currentScore = _segments[_currentSegment].RecordingScore;
            return currentScore > _personalBestScore;
        }

        public ISplits GetOutput()
        {
            var newSplits = new Splits() {
                GameId = _gameId,
                SplitName = _splitsName
            };

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
