using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Managers.SplitsBuilder;
using TouhouSplits.UI.Model;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Model
{
    public class PersonalBestTrackerTests
    {
        private static ISplitsBuilder GetDefaultSplitsBuilder(int numSegments)
        {
            var builder = Substitute.For<ISplitsBuilder>();
            builder.Segments.Returns(new List<IPersonalBestSegment>());
            for (int i = 0; i < numSegments; i++) {
                builder.Segments.Add(Substitute.For<IPersonalBestSegment>());
            }
            builder
                .When(n => n.SplitToNextSegment())
                .Do(n => builder.CurrentSegment.Returns(builder.CurrentSegment + 1));
            return builder;
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_RecordingSplits()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("RecordingSplits"));
        }

        [Fact]
        public void LoadPersonalBest_Loads_Matching_GameManager_If_Manager_Is_Not_Already_Loaded()
        {
            var facadeMock = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facadeMock);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            facadeMock.Received().LoadGameManager(new GameId("Game Id"));
        }

        [Fact]
        public void LoadPersonalBest_Sets_GameName_Using_Passed_In_Builder()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);

            facade.LoadGameManager(new GameId("Game Id")).GameName.Returns("Game Name");
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Game Name", model.GameName);
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_GameName()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("GameName"));
        }

        [Fact]
        public void LoadPersonalBest_Sets_SplitsName_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Splits Name", model.SplitsName);
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_SplitsName()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("SplitsName"));
        }

        [Fact]
        public void LoadPersonalBets_Sets_RecordingSplits_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            var builder = GetDefaultSplitsBuilder(1);
            var expectedSegments = builder.Segments;
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            Assert.Equal(expectedSegments, model.RecordingSplits);
        }

        [Fact]
        public void FavoriteSplits_Returns_List_From_GameManager()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);
            var expectedFavoriteSplits = new List<IFileHandler<ISplits>>();

            facade.LoadGameManager(new GameId("Game Id")).FavoriteSplits.Returns(expectedFavoriteSplits);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(expectedFavoriteSplits, model.FavoriteSplits());
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.StopScorePoller();
            Assert.Equal(-1, model.CurrentScore);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_IsNewPersonalBest_Returns_Value_From_PersonalBestBuilder(bool isNewPb)
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(1);
            builderMock.IsNewPersonalBest().Returns(isNewPb);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);

            builderMock.ClearReceivedCalls();
            Assert.Equal(isNewPb, model.IsNewPersonalBest);
            builderMock.Received().IsNewPersonalBest();
        }

        [Fact]
        public void IsPolling_Is_False_After_Construction()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_True()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Assert.Equal(true, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Resets_Builder_If_Polling_Is_Not_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(1);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            builderMock.Received().Reset();
        }

        [Fact]
        public void StartScorePoller_Does_Not_Reset_Builder_If_Polling_Is_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(1);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            builderMock.ClearReceivedCalls();
            model.StartScorePoller();
            builderMock.DidNotReceive().Reset();
        }

        [Fact]
        public void StartScorePoller_Fires_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Not_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StartScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_False()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Causes_Loaded_Builder_To_Continuously_Set_CurrentScore()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);

            var builderMock = GetDefaultSplitsBuilder(1);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            facade.LoadGameManager(new GameId("Game Id")).Hook.GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.Received().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StartScorePoller_Starts_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.True(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void StartScorePoller_Starts_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_IsNewPersonalBest()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.True(eventCatcher.CaughtProperties.Contains("IsNewPersonalBest"));
        }

        [Fact]
        public void SplitToNextSegment_Fires_Method_In_Builder_If_Polling_Is_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(2);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);

            model.StartScorePoller();
            builderMock.ClearReceivedCalls();
            model.SplitToNextSegment();
            builderMock.Received().SplitToNextSegment();
        }

        [Fact]
        public void SplitToNextSegment_Does_Not_Fire_Method_In_Builder_If_Polling_Is_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(2);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);

            model.StopScorePoller();
            builderMock.ClearReceivedCalls();
            model.SplitToNextSegment();
            builderMock.DidNotReceive().SplitToNextSegment();
        }

        [Fact]
        public void SplitToNextSegment_StopsPolling_If_Called_At_Last_Segment()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(2));

            model.StartScorePoller();
            model.SplitToNextSegment();
            model.SplitToNextSegment();
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StopScorePoller_Fires_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Not_Already_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StopScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StopScorePoller_Causes_Loaded_Builder_To_Stop_Setting_CurrentScore()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);

            var builderMock = GetDefaultSplitsBuilder(1);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            model.StopScorePoller();
            facade.LoadGameManager(new GameId("Game Id")).Hook.GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.DidNotReceive().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StopScorePoller_Fires_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();

            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StopScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void StopScorePoller_Stops_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.False(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void StopScorePoller_Stops_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_IsNewPersonalBest()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.False(eventCatcher.CaughtProperties.Contains("IsNewPersonalBest"));
        }

        [Fact]
        public void StopScorePoller_Returns_Output_Of_Builder_If_Polling_Is_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builder = GetDefaultSplitsBuilder(1);
            var expectedSplits = Substitute.For<ISplits>();
            builder.GetOutput().Returns(expectedSplits);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            model.StartScorePoller();
            Assert.Equal(expectedSplits, model.StopScorePoller());
        }

        [Fact]
        public void StopScorePoller_Returns_Output_Of_Builder_If_Polling_Is_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builder = GetDefaultSplitsBuilder(1);
            var expectedSplits = Substitute.For<ISplits>();
            builder.GetOutput().Returns(expectedSplits);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            Assert.Equal(expectedSplits, model.StopScorePoller());
        }
    }
}
