using TouhouSplits.MVVM;
using TouhouSplits.Service.Managers.SplitsBuilder;

namespace TouhouSplits.UI.Model
{
    public class PersonalBestSegment : ModelBase, IPersonalBestSegment
    {
        public PersonalBestSegment()
        {
            SegmentName = string.Empty;
            PersonalBestScore = -1;
            RecordingScore = -1;
        }

        private string _segmentName;
        public string SegmentName {
            get { return _segmentName; }
            set {
                _segmentName = value;
                NotifyPropertyChanged("SegmentName");
            }
        }

        private long _personalBestScore;
        public long PersonalBestScore {
            get { return _personalBestScore; }
            set {
                _personalBestScore = value;
                NotifyPropertyChanged("PersonalBestScore");
            }
        }

        private long _recordingScore;
        public long RecordingScore {
            get { return _recordingScore; }
            set {
                _recordingScore = value;
                NotifyPropertyChanged("RecordingScore");
            }
        }
    }
}
