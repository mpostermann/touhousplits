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
            IsCompleted = false;
        }

        private string _segmentName;
        public string SegmentName {
            get { return _segmentName; }
            private set {
                _segmentName = value;
                NotifyPropertyChanged(nameof(SegmentName));
            }
        }

        private long _personalBestScore;
        public long PersonalBestScore {
            get { return _personalBestScore; }
            private set {
                _personalBestScore = value;
                NotifyPropertyChanged(nameof(PersonalBestScore));
            }
        }

        private long _recordingScore;
        public long RecordingScore {
            get { return _recordingScore; }
            set {
                _recordingScore = value;
                NotifyPropertyChanged(nameof(RecordingScore));
            }
        }

        private bool _isRunning;
        public bool IsRunning {
            get { return _isRunning; }
            set {
                _isRunning = value;
                NotifyPropertyChanged(nameof(IsRunning));
            }
        }

        private bool _isCompleted;
        public bool IsCompleted {
            get { return _isCompleted; }
            set {
                _isCompleted = value;
                NotifyPropertyChanged(nameof(IsCompleted));
            }
        }
    }
}
