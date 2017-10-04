
namespace TouhouSplits.Service.Managers.SplitsBuilder
{
    public interface IPersonalBestSegment
    {
        string SegmentName { get; set; }
        long PersonalBestScore { get; set; }
        long RecordingScore { get; set; }
    }
}
