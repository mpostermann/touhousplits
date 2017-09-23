using System.Runtime.Serialization;
using TouhouSplits.MVVM;

namespace TouhouSplits.Service.Data
{
    [DataContract]
    public class Segment : ModelBase, ISegment
    {
        [DataMember]
        private string _segmentName;
        public string SegmentName {
            get { return _segmentName; }
            set {
                _segmentName = value;
                NotifyPropertyChanged("SegmentName");
            }
        }

        [DataMember]
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
