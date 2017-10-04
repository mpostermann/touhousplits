using TouhouSplits.MVVM;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.SplitsBuilder;

namespace TouhouSplits.UI.Model
{
    public class PersonalBestSegment : ModelBase, IPersonalBestSegment
    {
        public PersonalBestSegment(ISegment personalBest)
        {
            SegmentName = personalBest.SegmentName;
            PersonalBestScore = personalBest.Score;
            RecordingScore = -1;
        }

        private string _segmentName;
        public string SegmentName {
            get { return _segmentName; }
            private set {
                _segmentName = value;
                NotifyPropertyChanged("SegmentName");
            }
        }

        private long _personalBestScore;
        public long PersonalBestScore {
            get { return _personalBestScore; }
            private set {
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
