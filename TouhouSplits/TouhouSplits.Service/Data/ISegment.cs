
namespace TouhouSplits.Service.Data
{
    public interface ISegment
    {
        string SegmentName { get; }
        string ParentGameName { get; }
        long Score { get; }
    }
}
