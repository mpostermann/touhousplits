using NSubstitute;
using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers;
using TouhouSplits.Service.Managers.SplitsBuilder;
using TouhouSplits.UI.Model;
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
        public void CurrentSegment_Is_Set_To_0_By_Default()
        {
            var pb = CreateDefaultSplits(1);
            var builder = new PersonalBestSplitsBuilder(pb);

            Assert.Equal(0, builder.CurrentSegment);
        }

        [Fact]
        public void Segments_Are_Not_Running_Or_Completed_By_Default() {
            var pb = CreateDefaultSplits(10);
            var builder = new PersonalBestSplitsBuilder(pb);

            foreach (IPersonalBestSegment segment in builder.Segments) {
                Assert.Equal(false, segment.IsRunning);
                Assert.Equal(false, segment.IsCompleted);
            }
        }

        [Fact]
        public void MarkAsStarted_Sets_IsRunning_For_CurrentSegment() {
            var pb = CreateDefaultSplits(2);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.MarkAsStarted();
            Assert.Equal(true, builder.Segments[0].IsRunning);
            Assert.Equal(false, builder.Segments[0].IsCompleted);
            Assert.Equal(false, builder.Segments[1].IsRunning);
            Assert.Equal(false, builder.Segments[1].IsCompleted);
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

            for (int i = 0; i < numSegments - 1; i++) {
                builder.SplitToNextSegment();
            }
            builder.SetScoreForCurrentSegment(12345);
            Assert.Equal(12345, builder.Segments[numSegments - 1].RecordingScore);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void SplitToNextSegment_Increments_CurrentSegment_By_One(int numberOfTimesToSplit)
        {
            var pb = CreateDefaultSplits(numberOfTimesToSplit + 1);
            var builder = new PersonalBestSplitsBuilder(pb);

            for (int i = 0; i < numberOfTimesToSplit; i++) {
                builder.SplitToNextSegment();
            }
            Assert.Equal(numberOfTimesToSplit, builder.CurrentSegment);
        }

        [Fact]
        public void SplitToNextSegment_Marks_Segment_As_Completed_And_Next_Segment_As_Running()
        {
            var pb = CreateDefaultSplits(2);
            var builder = new PersonalBestSplitsBuilder(pb);

            Assert.Equal(false, builder.Segments[0].IsCompleted);

            builder.SplitToNextSegment();
            Assert.Equal(true, builder.Segments[0].IsCompleted);
            Assert.Equal(false, builder.Segments[0].IsRunning);
            Assert.Equal(false, builder.Segments[1].IsCompleted);
            Assert.Equal(true, builder.Segments[1].IsRunning);
        }

        [Fact]
        public void SplitToNextSegment_Does_Nothing_If_Called_More_Times_Than_The_Count_Of_Segments_In_Personal_Best()
        {
            var pb = CreateDefaultSplits(3);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SplitToNextSegment();
            builder.SplitToNextSegment();

            // Split a 3rd time, but expect that we don't advance to a new segment (since we're already at the last one)
            builder.SplitToNextSegment();
            Assert.Equal(2, builder.CurrentSegment);
        }

        [Fact]
        public void Reset_Causes_Next_SetScoreForCurrentSegment_To_Set_To_The_1st_Segment()
        {
            var pb = CreateDefaultSplits(3);
            var builder = new PersonalBestSplitsBuilder(pb);

            builder.SplitToNextSegment();
            builder.Reset();
            builder.SetScoreForCurrentSegment(12345);
            Assert.Equal(12345, builder.Segments[0].RecordingScore);
        }

        [Fact]
        public void Reset_Causes_Previously_Set_Segments_To_Be_Set_To_Their_Default_Value()
        {
            var pb = CreateDefaultSplits(3);
            var builder = new PersonalBestSplitsBuilder(pb);
            
            builder.MarkAsStarted();
            builder.SetScoreForCurrentSegment(1);
            builder.SplitToNextSegment();
            builder.SetScoreForCurrentSegment(2);
            builder.SplitToNextSegment();
            builder.SetScoreForCurrentSegment(3);

            builder.Reset();
            Assert.Equal(Constants.UNSET_SCORE, builder.Segments[0].RecordingScore);
            Assert.Equal(false, builder.Segments[0].IsRunning);
            Assert.Equal(false, builder.Segments[0].IsCompleted);
            Assert.Equal(Constants.UNSET_SCORE, builder.Segments[1].RecordingScore);
            Assert.Equal(false, builder.Segments[1].IsRunning);
            Assert.Equal(false, builder.Segments[1].IsCompleted);
            Assert.Equal(Constants.UNSET_SCORE, builder.Segments[2].RecordingScore);
            Assert.Equal(false, builder.Segments[1].IsRunning);
            Assert.Equal(false, builder.Segments[1].IsCompleted);
        }

        [Fact]
        public void MarkAsStopped_Sets_Current_Segment_To_Completed_And_No_Segment_As_Running() {
            var pb = CreateDefaultSplits(3);
            var builder = new PersonalBestSplitsBuilder(pb);
            
            builder.MarkAsStarted();
            builder.SetScoreForCurrentSegment(1);
            builder.SplitToNextSegment();
            builder.MarkAsStopped();

            Assert.Equal(false, builder.Segments[0].IsRunning);
            Assert.Equal(true, builder.Segments[0].IsCompleted);
            Assert.Equal(false, builder.Segments[1].IsRunning);
            Assert.Equal(true, builder.Segments[1].IsCompleted);
            Assert.Equal(false, builder.Segments[2].IsRunning);
            Assert.Equal(false, builder.Segments[2].IsCompleted);
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

        [Fact]
        public void GetOutput_GameID_Is_Equal_To_Personal_Best_GameID()
        {
            var pb = CreateDefaultSplits(1);
            pb.GameId.Returns(new GameId("Some game id"));
            var builder = new PersonalBestSplitsBuilder(pb);

            var newSplits = builder.GetOutput();
            Assert.Equal(new GameId("Some game id"), newSplits.GameId);
        }

        [Fact]
        public void GetOutput_SplitName_Is_Equal_To_Personal_Best_SplitName()
        {
            var pb = CreateDefaultSplits(1);
            pb.SplitName.Returns("Some split name");
            var builder = new PersonalBestSplitsBuilder(pb);

            var newSplits = builder.GetOutput();
            Assert.Equal("Some split name", newSplits.SplitName);
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

            for (int i = 0; i < numSegments - 1; i++) {
                builder.SetScoreForCurrentSegment(i * 2);
                builder.SplitToNextSegment();
            }
            builder.SetScoreForCurrentSegment( (numSegments - 1) * 2);
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
