
namespace TouhouSplits.Service.Managers.SplitsBuilder
{
    public interface IPersonalBestSegment
    {
        string SegmentName { get; }
        long PersonalBestScore { get; }
        long RecordingScore { get; }
        bool SegmentCompleted { get; }
    }
}
