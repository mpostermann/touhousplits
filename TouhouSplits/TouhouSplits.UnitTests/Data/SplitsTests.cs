using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.UnitTests.Data
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
            string expectedSplitName = "Some splits name";
            splits.SplitName = expectedSplitName;
            Assert.Equal(expectedSplitName, splits.GameName);
        }

        [Fact]
        public void SplitNameIsSetWithCorrectValue()
        {
            string changedPropertyName = null;
            var splits = new Splits();
            splits.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedPropertyName = e.PropertyName;
            };

            splits.SplitName = "Some split name";
            Assert.Equal("SplitName", changedPropertyName);
        }


        [Fact]
        public void SettingSplitNameFiresPropertyChangedEvent()
        {
            string changedPropertyName = null;
            var splits = new Splits();
            splits.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedPropertyName = e.PropertyName;
            };

            splits.SplitName = "Some split name";
            Assert.Equal("SplitName", changedPropertyName);
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
            splits.AddSegment(0, segment1);
            Assert.Equal(segment0, splits.Segments[0]);
            Assert.Equal(segment1, splits.Segments[1]);
        }

        [Fact]
        public void ExceptionIsThrowWhenAddingSegmentOutOfBounds()
        {
            var splits = new Splits();
            var segment = Substitute.For<ISegment>();

            Assert.Throws<IndexOutOfRangeException>(() => splits.AddSegment(1, segment));
        }

        [Fact]
        public void AddingSegmentFiresSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RemoveSegmentRemovesSegmentFromList()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [Fact]
        public void CorrectSegmentIsRemovedWhenRemovingFromBeginningOfList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ExceptionIsThrowWhenRemovingSegmentOutOfBounds()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RemovingSegmentFiresSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RemovingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RemovingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CorrectSegmentIsUpdatedWhenUpdatingMiddleOfList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CorrectSegmentIsUpdatedWhenUpdatingEndOfList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CorrectSegmentIsUpdatedWhenUpdatingBeginningOfList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ExceptionIsThrowWhenUpdatingSegmentOutOfBounds()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatingSegmentFiresSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdaingSegmentAtEndOfListFiresEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatingSegmentInMiddleOfListDoesNotFireEndingSegmentPropertyChangedEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void EndingSegmentIsNullWhenSegmentsListIsEmpty()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void EndingSegmentReturnsCorrectSegmentWhenSegmentsListIsNonEmpty()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CloneReturnsANewInstance()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CloneHasTheSameGameName()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CloneHasTheSameSplitName()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CloneHasADifferentSegmentsListInstance()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CloneHasSegmentsListWithEquivalentSegments()
        {
            throw new NotImplementedException();
        }
    }
}
