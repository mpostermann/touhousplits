using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TouhouSplits.MVVM;

namespace TouhouSplits.Service.Data
{
    [DataContract, KnownType(typeof(Segment))]
    public class Splits : ModelBase, ISplits
    {
        public Splits()
        {
            GameId = new GameId();
            _segments = new List<ISegment>();
        }

        [DataMember]
        private GameId _gameId;
        public GameId GameId {
            get { return _gameId; }
            set {
                _gameId = value;
                NotifyPropertyChanged("GameId");
            }
        }

        [DataMember]
        private string _splitName;
        public string SplitName {
            get { return _splitName; }
            set {
                _splitName = value;
                NotifyPropertyChanged("SplitName");
            }
        }

        [DataMember]
        private List<ISegment> _segments;
        public IList<ISegment> Segments { get { return _segments.AsReadOnly(); } }

        public ISegment EndingSegment {
            get {
                if (_segments == null || _segments.Count == 0) {
                    return null;
                }
                int index = _segments.Count - 1;
                return _segments[index];
            }
        }

        public void AddSegment(int index, ISegment segment)
        {
            _segments.Insert(index, segment);

            NotifyPropertyChanged("Segments");
            if (index == _segments.Count - 1) {
                NotifyPropertyChanged("EndingSegment");
            }
        }

        public void RemoveSegment(int index)
        {
            _segments.RemoveAt(index);

            NotifyPropertyChanged("Segments");
            if (index == _segments.Count) {
                NotifyPropertyChanged("EndingSegment");
            }
        }

        public void UpdateSegment(int index, ISegment segment)
        {
            _segments[index] = segment;

            NotifyPropertyChanged("Segments");
            if (index == _segments.Count - 1) {
                NotifyPropertyChanged("EndingSegment");
            }
        }

        public virtual object Clone()
        {
            var clone = new Splits();
            clone.GameId = this.GameId;
            clone.SplitName = this.SplitName;
            foreach (ISegment segment in this._segments) {
                clone._segments.Add(new Segment() {
                    SegmentName = segment.SegmentName,
                    Score = segment.Score
                });
            }
            return clone;
        }

    }
}
