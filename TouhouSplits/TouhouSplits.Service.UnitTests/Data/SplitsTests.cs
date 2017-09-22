using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TouhouSplits.Service.Data;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SplitsTests
    {
        [Fact]
        public void IsInitializedWithDefaultGameName() 
        {
            var splits = new Splits();
            Assert.Equal(null, splits.GameName);
        }

        [Fact]
        public void IsInitializedWithDefaultSplitName()
        {
            var splits = new Splits();
            Assert.Equal(null, splits.SplitName);
        }

        [Fact]
        public void IsInitializedWithEmptySegmentsList()
        {
            var splits = new Splits();
            Assert.Equal(0, splits.Segments.Count);
        }

        [Fact]
        public void GameNameIsSetWithCorrectValue()
        {
            var splits = new Splits();
            string expectedGameName = "Some game name";
            splits.GameName = expectedGameName;
            Assert.Equal(expectedGameName, splits.GameName);
        }

        [Fact]
        public void SettingGameNameFiresPropertyChangedEvent()
        {
            var splits = new Splits();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.GameName = "Some game name";
            Assert.True(eventCatcher.CaughtProperties.Contains("GameName"));
        }

        [Fact]
        public void SplitNameIsSetWithCorrectValue()
        {
            var splits = new Splits();
            string expectedSplitName = "Some splits name";
            splits.SplitName = expectedSplitName;
            Assert.Equal(expectedSplitName, splits.SplitName);
        }


        [Fact]
        public void SettingSplitNameFiresPropertyChangedEvent()
        {
            var splits = new Splits();
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.SplitName = "Some split name";
            Assert.True(eventCatcher.CaughtProperties.Contains("SplitName"));
        }

        [Fact]
        public void SegmentIsAddedToEmptyList()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();

            splits.AddSegment(0, segment);
            Assert.Equal(segment, splits.Segments[0]);
        }

        [Fact]
        public void SegmentIsAddedInCorrectOrderWhenInsertedInMiddleOfList()
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
        public void SegmentIsAddedInCorrectOrderWhenInsertedAtStartOfList()
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
        public void SegmentIsAddedInCorrectOrderWhenInsertedAtEndOfList()
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
        public void ExceptionIsThrowWhenAddingSegmentOutOfBounds()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.AddSegment(1, segment));
        }

        [Fact]
        public void AddingSegmentFiresSegmentPropertyChangedEvent()
        {
            var eventCatcher = new NotifyPropertyChangedCatcher();
            var splits = new Splits();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void AddingSegmentToEmptyListFiresEndingSegmentPropertyChangedEvent()
        {
            var eventCatcher = new NotifyPropertyChangedCatcher();
            var splits = new Splits();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void AddingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.AddSegment(1, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void AddingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
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
        public void RemoveSegmentRemovesSegmentFromList()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            splits.RemoveSegment(0);
            Assert.Equal(0, splits.Segments.Count);
        }

        [Fact]
        public void CorrectSegmentIsRemovedWhenRemovingFromMiddleOfList()
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
        public void CorrectSegmentIsRemovedWhenRemovingFromEndOfList()
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
        public void CorrectSegmentIsRemovedWhenRemovingFromBeginningOfList()
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
        public void ExceptionIsThrowWhenRemovingSegmentOutOfBounds()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.RemoveSegment(1));
        }

        [Fact]
        public void RemovingSegmentFiresSegmentPropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.RemoveSegment(0);
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void RemovingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.RemoveSegment(0);
            Assert.True(eventCatcher.CaughtProperties.Contains("EndingSegment"));
        }

        [Fact]
        public void RemovingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
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
        public void CorrectSegmentIsUpdatedWhenUpdatingMiddleOfList()
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
        public void CorrectSegmentIsUpdatedWhenUpdatingEndOfList()
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
        public void CorrectSegmentIsUpdatedWhenUpdatingBeginningOfList()
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
        public void ExceptionIsThrowWhenUpdatingSegmentOutOfBounds()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            Assert.Throws<ArgumentOutOfRangeException>(() => splits.UpdateSegment(1, Substitute.For<ISegment>()));
        }

        [Fact]
        public void UpdatingSegmentFiresSegmentPropertyChangedEvent()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            splits.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            splits.UpdateSegment(0, Substitute.For<ISegment>());
            Assert.True(eventCatcher.CaughtProperties.Contains("Segments"));
        }

        [Fact]
        public void UpdaingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
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
        public void UpdatingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
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
        public void EndingSegmentIsNullWhenSegmentsListIsEmpty()
        {
            var splits = new Splits();
            Assert.Equal(null, splits.EndingSegment);
        }

        [Fact]
        public void EndingSegmentReturnsCorrectSegmentWhenSegmentsListIsNonEmpty()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            var endingSegment = Substitute.For<ISegment>();
            splits.AddSegment(1, endingSegment);
            Assert.Equal(endingSegment, splits.EndingSegment);
        }

        [Fact]
        public void CloneReturnsANewInstance()
        {
            var splits = new Splits();
            var clone = splits.Clone();
            Assert.NotEqual(splits, clone);
        }

        [Fact]
        public void CloneHasTheSameGameName()
        {
            var splits = new Splits();
            string gameName = "Some game name";
            splits.GameName = gameName;

            var clone = splits.Clone();
            Assert.Equal(gameName, clone.GameName);
        }

        [Fact]
        public void CloneHasTheSameSplitName()
        {
            var splits = new Splits();
            string splitName = "Some split name";
            splits.SplitName = splitName;

            var clone = splits.Clone();
            Assert.Equal(splitName, clone.SplitName);
        }

        [Fact]
        public void CloneHasADifferentSegmentsListInstance()
        {
            var splits = new Splits();
            splits.AddSegment(0, Substitute.For<ISegment>());

            var clone = splits.Clone();
            Assert.NotEqual(splits.Segments, clone.Segments);
            Assert.NotEqual(splits.Segments[0], clone.Segments[0]);
        }

        [Fact]
        public void CloneHasSegmentsListWithEquivalentSegments()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();
            segment.Score = 1561;
            segment.SegmentName = "Some segment";
            splits.AddSegment(0, segment);

            var clone = splits.Clone();
            Assert.Equal(segment.SegmentName, clone.Segments[0].SegmentName);
            Assert.Equal(segment.Score, clone.Segments[0].Score);
        }
    }
}
