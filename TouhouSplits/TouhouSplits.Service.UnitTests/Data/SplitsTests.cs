using NSubstitute;
using System;
using TouhouSplits.Service.Data;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SplitsTests
    {
        [Fact]
        public void Default_GameName_Is_Null() 
        {
            var splits = new Splits();
            Assert.Equal(null, splits.GameName);
        }

        [Fact]
        public void Default_SplitName_Is_Null()
        {
            var splits = new Splits();
            Assert.Equal(null, splits.SplitName);
        }

        [Fact]
        public void Default_Segments_List_Is_Empty()
        {
            var splits = new Splits();
            Assert.Equal(0, splits.Segments.Count);
        }

        [Fact]
        public void Get_GameName_Returns_Last_Set_Value()
        {
            var splits = new Splits();
            string expectedGameName = "Some game name";
            splits.GameName = expectedGameName;
            Assert.Equal(expectedGameName, splits.GameName);
        }

        [Fact]
        public void Set_GameName_Fires_PropertyChangedEvent()
        {
            var splits = new Splits();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.GameName = "Some game name";
            Assert.True(eventCatcher.CaughtProperties.Contains("GameName"));
        }

        [Fact]
        public void Get_SplitName_Returns_Last_Set_Value()
        {
            var splits = new Splits();
            string expectedSplitName = "Some splits name";
            splits.SplitName = expectedSplitName;
            Assert.Equal(expectedSplitName, splits.SplitName);
        }


        [Fact]
        public void Set_SplitName_Fires_PropertyChangedEvent()
        {
            var splits = new Splits();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.SplitName = "Some split name";
            Assert.True(eventCatcher.CaughtProperties.Contains("SplitName"));
        }

        [Fact]
        public void AddSegment_Adds_Segment_To_Empty_List()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();

            splits.AddSegment(0, segment);
            Assert.Equal(segment, splits.Segments[0]);
        }

        [Fact]
        public void AddSegment_Inserts_Segment_When_Added_In_Middle_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();

            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment2);
            splits.AddSegment(1, segment1);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
            Assert.Equal(segment2, splits.Segments[2]);
        }

        [Fact]
        public void AddSegment_Inserts_Segment_When_Inserted_At_Start_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();

            splits.AddSegment(0, segment1);
            splits.AddSegment(0, segment0);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
        }

        [Fact]
        public void AddSegment_Adds_Segment_To_The_End_When_Added_At_End_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();

            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment1);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
        }

        [Fact]
        public void AddSegment_Throws_Exception_When_Index_Is_Out_Of_Bounds()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.AddSegment(1, segment));
        }

        [Fact]
        public void AddSegment_Fires_Segment_PropertyChangedEvent()
        {
            var eventCatcher = new NotifyPropertyChangedCatcher();
            var splits = new Splits();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void AddSegment_To_Empty_List_Fires_EndingSegment_PropertyChangedEvent()
        {
            var eventCatcher = new NotifyPropertyChangedCatcher();
            var splits = new Splits();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void AddSegment_At_End_Of_List_Fires_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(1, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void AddSegment_In_Middle_Of_List_Does_Not_Fire_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            splits.AddSegment(1, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(1, Substitute.For<ISegment>());
            Assert.False(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void RemoveSegment_Removes_Segment_From_List()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            splits.RemoveSegment(0);
            Assert.Equal(0, splits.Segments.Count);
        }

        [Fact]
        public void RemoveSegment_Removes_Segment_From_Middle_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();
            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment1);
            splits.AddSegment(2, segment2);

            splits.RemoveSegment(1);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment2, splits.Segments[1]);
        }

        [Fact]
        public void RemoveSegment_Removes_Segment_From_End_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();
            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment1);
            splits.AddSegment(2, segment2);

            splits.RemoveSegment(2);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
        }

        [Fact]
        public void RemoveSegment_Removes_Segment_From_Beginning_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();
            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment1);
            splits.AddSegment(2, segment2);

            splits.RemoveSegment(0);
            Assert.Equal(segment1, splits.Segments[0]);
            Assert.Equal(segment2, splits.Segments[1]);
        }

        [Fact]
        public void RemoveSegment_Throws_Exception_When_Index_Is_Out_Of_Bounds()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.RemoveSegment(1));
        }

        [Fact]
        public void RemoveSegment_First_Segment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.RemoveSegment(0);
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void RemoveSegment_At_End_Of_List_First_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.RemoveSegment(0);
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void RemoveSegment_In_Middle_Of_List_Does_Not_Fire_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            splits.AddSegment(1, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.RemoveSegment(0);
            Assert.False(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void UpdateSegment_Updates_Segment_In_Middle_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();
            splits.AddSegment(0, segment0);
            splits.AddSegment(1, Substitute.For<ISegment>());
            splits.AddSegment(2, segment2);

            var segment1 = Substitute.For<ISegment>();
            splits.UpdateSegment(1, segment1);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
            Assert.Equal(segment2, splits.Segments[2]);
        }

        [Fact]
        public void UpdateSegment_Updates_Segment_At_End_Of_List()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            var segment1 = Substitute.For<ISegment>();
            splits.AddSegment(0, segment0);
            splits.AddSegment(1, segment1);
            splits.AddSegment(2, Substitute.For<ISegment>());
            
            var segment2 = Substitute.For<ISegment>();
            splits.UpdateSegment(2, segment2);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
            Assert.Equal(segment2, splits.Segments[2]);
        }

        [Fact]
        public void UpdateSegment_Updates_Segment_At_Beginning_Of_List()
        {
            var splits = new Splits();
            var segment1 = Substitute.For<ISegment>();
            var segment2 = Substitute.For<ISegment>();
            splits.AddSegment(0, Substitute.For<ISegment>());
            splits.AddSegment(1, segment1);
            splits.AddSegment(2, segment2);

            var segment0 = Substitute.For<ISegment>();
            splits.UpdateSegment(0, segment0);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
            Assert.Equal(segment2, splits.Segments[2]);
        }

        [Fact]
        public void UpdateSegment_Throws_Exception_When_Index_Is_Out_Of_Bounds()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.UpdateSegment(1, Substitute.For<ISegment>()));
        }

        [Fact]
        public void UpdateSegment_Fires_Segment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.UpdateSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void UpdateSegment_At_End_Of_List_Fires_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            splits.AddSegment(1, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.UpdateSegment(1, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void UpdateSegment_In_Middle_Of_List_Does_Not_Fire_EndingSegment_PropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            splits.AddSegment(1, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.UpdateSegment(0, Substitute.For<ISegment>());
            Assert.False(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void EndingSegment_Is_Null_When_Segments_List_Is_Empty()
        {
            var splits = new Splits();
            Assert.Equal(null, splits.EndingSegment);
        }

        [Fact]
        public void EndingSegment_Returns_Segment_At_End_Of_List()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            var endingSegment = Substitute.For<ISegment>();
            splits.AddSegment(1, endingSegment);
            Assert.Equal(endingSegment, splits.EndingSegment);
        }

        [Fact]
        public void Clone_Returns_A_New_Instance()
        {
            var splits = new Splits();
            var clone = splits.Clone();
            Assert.NotEqual(splits, clone);
        }

        [Fact]
        public void Clone_Has_The_Same_GameName()
        {
            var splits = new Splits();
            splits.GameName = "Some game name";

            var clone = (Splits) splits.Clone();
            Assert.Equal("Some game name", clone.GameName);
        }

        [Fact]
        public void Clone_Has_The_Same_SplitName()
        {
            var splits = new Splits();
            splits.SplitName = "Some split name";

            var clone = (Splits) splits.Clone();
            Assert.Equal("Some split name", clone.SplitName);
        }

        [Fact]
        public void Clone_Has_A_New_Segments_List_Instance()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            var clone = (Splits) splits.Clone();
            Assert.NotEqual(splits.Segments, clone.Segments);
            Assert.NotEqual(splits.Segments[0], clone.Segments[0]);
        }

        [Fact]
        public void Clone_Has_Segments_List_With_Equivalent_Segments()
        {
            var splits = new Splits();
            var segment0 = Substitute.For<ISegment>();
            segment0.Score = 1561;
            segment0.SegmentName = "Some segment";
            splits.AddSegment(0, segment0);
            var segment1 = Substitute.For<ISegment>();
            segment1.Score = 897116;
            segment1.SegmentName = "Some segment stage 2";
            splits.AddSegment(1, segment1);


            var clone = (Splits) splits.Clone();
            Assert.Equal("Some segment", clone.Segments[0].SegmentName);
            Assert.Equal(1561, clone.Segments[0].Score);
            Assert.Equal("Some segment stage 2", clone.Segments[1].SegmentName);
            Assert.Equal(897116, clone.Segments[1].Score);
        }
    }
}
