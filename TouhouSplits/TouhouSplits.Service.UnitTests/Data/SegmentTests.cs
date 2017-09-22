using System.ComponentModel;
using TouhouSplits.Service.Data;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SegmentTests
    {
        [Fact]
        public void Default_Score_Is_Zero()
        {
            var segment = new Segment();
            Assert.Equal(0, segment.Score);
        }

        [Fact]
        public void Default_SegmentName_Is_Null()
        {
            var segment = new Segment();
            Assert.Equal(null, segment.SegmentName);
        }

        [Fact]
        public void Get_Score_Returns_Last_Set_Value()
        {
            var segment = new Segment();
            long expectedValue = 15676;

            segment.Score = expectedValue;
            Assert.Equal(expectedValue, segment.Score);
        }

        [Fact]
        public void Set_Score_Fires_PropertyChangedEvent()
        {
            var segment = new Segment();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            segment.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            segment.Score = 1;
            Assert.True(eventCatcher.CaughtProperties.Contains("Score"));
        }

        [Fact]
        public void Get_Name_Returns_Last_Set_Value()
        {
            var segment = new Segment();
            string expectedName = "I am a segment. ";

            segment.SegmentName = expectedName;
            Assert.Equal(expectedName, segment.SegmentName);
        }

        [Fact]
        public void Set_Name_Fires_PropertyChangedEvent()
        {
            var segment = new Segment();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            segment.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            segment.SegmentName = "name";
            Assert.True(eventCatcher.CaughtProperties.Contains("SegmentName"));
        }
    }
}
