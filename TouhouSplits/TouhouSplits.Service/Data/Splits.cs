using System.Collections.Generic;
using TouhouSplits.MVVM;

namespace TouhouSplits.Service.Data
{
    public class Splits : ModelBase, ISplits
    {
        public Splits()
        {
            _segments = new List<ISegment>();
        }

        private string _gameName;
        public string GameName {
            get { return _gameName; }
            set {
                _gameName = value;
                NotifyPropertyChanged("GameName");
            }
        }

        private string _splitName;
        public string SplitName {
            get { return _splitName; }
            set {
                _splitName = value;
                NotifyPropertyChanged("SplitName");
            }
        }

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

        public ISplits Clone()
        {
            var clone = new Splits();
            clone.GameName = this.GameName;
            clone.SplitName = this.SplitName;
            foreach (ISegment segment in this._segments) {
                clone._segments.Add(segment);
            }
            return clone;
        }

    }
}
