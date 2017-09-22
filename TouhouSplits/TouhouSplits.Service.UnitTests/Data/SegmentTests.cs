using System.ComponentModel;
using TouhouSplits.Service.Data;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SegmentTests
    {
        [Fact]
        public void SegmentIsInitializedWithDefaultScore()
        {
            var segment = new Segment();
            Assert.Equal(0, segment.Score);
        }

        [Fact]
        public void SegmentIsInitializedWithDefaultName()
        {
            var segment = new Segment();
            Assert.Equal(null, segment.SegmentName);
        }

        [Fact]
        public void ScoreIsSetWithCorrectValue()
        {
            var segment = new Segment();
            long expectedValue = 15676;

            segment.Score = expectedValue;
            Assert.Equal(expectedValue, segment.Score);
        }

        [Fact]
        public void SettingScoreFiresPropertyChangedEvent()
        {
            var segment = new Segment();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            segment.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            segment.Score = 1;
            Assert.True(eventCatcher.CaughtProperties.Contains("Score"));
        }

        [Fact]
        public void NameIsSetWithCorrectValue()
        {
            var segment = new Segment();
            string expectedName = "I am a segment. ";

            segment.SegmentName = expectedName;
            Assert.Equal(expectedName, segment.SegmentName);
        }

        [Fact]
        public void SettingNameFiresPropertyChangedEvent()
        {
            var segment = new Segment();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            segment.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            segment.SegmentName = "name";
            Assert.True(eventCatcher.CaughtProperties.Contains("SegmentName"));
        }
    }
}
