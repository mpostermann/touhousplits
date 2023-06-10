using TouhouSplits.MVVM;
using TouhouSplits.Service;
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
            RecordingScore = Constants.UNSET_SCORE;
            SegmentCompleted = false;
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

        private bool _segmentCompleted;
        public bool SegmentCompleted {
            get { return _segmentCompleted; }
            set {
                _segmentCompleted = value;
                NotifyPropertyChanged("SegmentCompleted");
            }
        }
    }
}
