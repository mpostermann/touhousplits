using NSubstitute;
using System;
using System.Collections.Generic;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Managers.SplitsBuilder
{
    public class PersonalBestSplitsBuilderTests
    {
        private static ISplits CreateDefaultSplits(int numSegments)
        {
            var pb = Substitute.For<ISplits>();
            pb.Segments.Returns(new List<ISegment>());
            for (int i = 0; i < numSegments; i++) {
                pb.Segments.Add(Substitute.For<ISegment>());
                pb.Segments[i].SegmentName.Returns($"Segment {i}");
                pb.Segments[i].Score = i;
            }
            return pb;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Segments_Count_Has_Same_Count_As_Personal_Best(int expectedCount)
        {
            var pb = CreateDefaultSplits(expectedCount);
            var builder = new PersonalBestSplitsBuilder(pb);

            Assert.Equal(expectedCount, builder.Segments.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void Segments_List_Has_Names_Equal_To_Personal_Best(int numSegments)
        {
            var pb = CreateDefaultSplits(numSegments);
            var builder = new PersonalBestSplitsBuilder(pb);

            for (int i = 0; i < numSegments; i++) {
                Assert.Equal($"Segment {i}", builder.Segments[i].SegmentName);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void Segments_List_Has_Score_Equal_To_Personal_Best(int numSegments)
        {
            var pb = CreateDefaultSplits(numSegments);
            var builder = new PersonalBestSplitsBuilder(pb);

            for (int i = 0; i < numSegments; i++) {
                Assert.Equal(i, builder.Segments[i].PersonalBestScore);
            }
        }

        [Fact]
        public void SetScoreForCurrentSegment_Updates_Score_For_First_Segment_Upon_Construction()
        {
            var pb = CreateDefaultSplits(2);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SetScoreForCurrentSegment(12345);
            Assert.Equal(12345, builder.Segments[0].RecordingScore);
        }

        [Fact]
        public void SetScoreForCurrentSegment_Updates_Score_For_First_Segment_Upon_Construction_For_Multiple_Calls()
        {
            var pb = CreateDefaultSplits(2);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SetScoreForCurrentSegment(12345);
            builder.SetScoreForCurrentSegment(67890);
            Assert.Equal(67890, builder.Segments[0].RecordingScore);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void SetScoreForCurrentSegment_Updates_Score_For_Nth_Segment_After_N_Calls_To_SplitToNextSegment(int numSegments)
        {
            var pb = CreateDefaultSplits(numSegments);
            var builder = new PersonalBestSplitsBuilder(pb);

            for (int i = 0; i < numSegments; i++) {
                builder.SplitToNextSegment();
            }
            builder.SetScoreForCurrentSegment(12345);
            Assert.Equal(12345, builder.Segments[numSegments].RecordingScore);
        }

        [Fact]
        public void SplitToNextSegment_Throws_Exception_If_Called_More_Times_Than_The_Count_Of_Segments_In_Personal_Best()
        {
            var pb = CreateDefaultSplits(2);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SplitToNextSegment();
            builder.SplitToNextSegment();
            Assert.Throws<InvalidOperationException>(() => builder.SplitToNextSegment());
        }

        [Fact]
        public void IsNewPersonalBest_Returns_True_If_Last_Set_Score_Is_Higher_Than_Ending_Score_Of_Personal_Best()
        {
            var pb = CreateDefaultSplits(1);
            pb.EndingSegment.Score.Returns(100);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SetScoreForCurrentSegment(101);
            Assert.Equal(true, builder.IsNewPersonalBest());
        }

        [Fact]
        public void IsNewPersonalBest_Returns_False_If_Last_Set_Score_Is_Lower_Than_Ending_Score_Of_Personal_Best()
        {
            var pb = CreateDefaultSplits(1);
            pb.EndingSegment.Score.Returns(100);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SetScoreForCurrentSegment(99);
            Assert.Equal(false, builder.IsNewPersonalBest());
        }

        [Fact]
        public void IsNewPersonalBest_Returns_False_If_Last_Set_Score_Is_Equal_To_Ending_Score_Of_Personal_Best()
        {
            var pb = CreateDefaultSplits(1);
            pb.EndingSegment.Score.Returns(100);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SetScoreForCurrentSegment(100);
            Assert.Equal(false, builder.IsNewPersonalBest());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void GetOutput_Segments_Has_Same_Count_Equal_As_Personal_Best(int expectedCount)
        {
            var pb = CreateDefaultSplits(expectedCount);
            var builder = new PersonalBestSplitsBuilder(pb);

            var newSplits = builder.GetOutput();
            Assert.Equal(expectedCount, newSplits.Segments.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void GetOutput_Segments_Has_Names_Equal_To_Personal_Best(int numSegments)
        {
            var pb = CreateDefaultSplits(numSegments);
            var builder = new PersonalBestSplitsBuilder(pb);

            var newSplits = builder.GetOutput();
            for (int i = 0; i < numSegments; i++) {
                Assert.Equal($"Segment {i}", newSplits.Segments[i].SegmentName);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void GetOutput_Segments_Has_Scores_Equal_To_Scores_Set_By_SetScoresForCurrentSegment(int numSegments)
        {
            var pb = CreateDefaultSplits(numSegments);
            var builder = new PersonalBestSplitsBuilder(pb);

            for (int i = 0; i < numSegments; i++) {
                builder.SetScoreForCurrentSegment(i * 2);
                builder.SplitToNextSegment();
            }
            var newSplits = builder.GetOutput();

            for (int i = 0; i < numSegments; i++) {
                Assert.Equal(i * 2, newSplits.Segments[i].Score);
            }
        }

        [Fact]
        public void GetOutput_Segments_Ends_With_Scores_Equal_To_Last_Set_Score_If_SplitToNextSegment_Is_Called_Less_Times_Than_The_Number_Of_Segments()
        {
            var pb = CreateDefaultSplits(4);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SplitToNextSegment();
            builder.SetScoreForCurrentSegment(100);
            var newSplits = builder.GetOutput();

            Assert.Equal(100, newSplits.Segments[2].Score);
            Assert.Equal(100, newSplits.Segments[3].Score);
        }
    }
}
