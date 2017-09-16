using TouhouSplits.MVVM;

namespace TouhouSplits.Service.Data
{
    public class Segment : ModelBase, ISegment
    {
        private string _segmentName;
        public string SegmentName {
            get { return _segmentName; }
            set {
                _segmentName = value;
                NotifyPropertyChanged("SegmentName");
            }
        }

        private long _score;
        public long Score {
            get { return _score; }
            set {
                _score = value;
                NotifyPropertyChanged("Score");
            }
        }
    }
}
