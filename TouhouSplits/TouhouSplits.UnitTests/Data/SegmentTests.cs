using System.ComponentModel;
using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.UnitTests.Data
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
            string changedPropertyName = null;
            var segment = new Segment();
            segment.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedPropertyName = e.PropertyName;
            };

            segment.Score = 1;
            Assert.Equal("Score", changedPropertyName);
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
            string changedPropertyName = null;
            var segment = new Segment();
            segment.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedPropertyName = e.PropertyName;
            };

            segment.SegmentName = "name";
            Assert.Equal("SegmentName", changedPropertyName);
        }
    }
}
