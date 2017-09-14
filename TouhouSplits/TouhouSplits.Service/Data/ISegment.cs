
namespace TouhouSplits.Service.Data
{
    public interface ISegment
    {
        string SegmentName { get; set; }
        long Score { get; set; }
    }
}
