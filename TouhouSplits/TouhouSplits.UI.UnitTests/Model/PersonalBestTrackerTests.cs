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

        private static ISplitsFacade DefaultSplitsFacade()
        {
            var facade = Substitute.For<ISplitsFacade>();
            facade.LoadGameManager(Arg.Any<GameId>()).GameIsRunning().Returns(true);
            return facade;
        }

        [Fact]
        public void HasError_Returns_False_By_Default()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            Assert.False(model.HasError);
        }

        [Fact]
        public void LastError_Returns_Null_By_Default()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            Assert.Null(model.LastError);
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_RecordingSplits()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("RecordingSplits"));
        }

        [Fact]
        public void LoadPersonalBest_Loads_Matching_GameManager_If_Manager_Is_Not_Already_Loaded()
        {
            var facadeMock = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facadeMock);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            facadeMock.Received().LoadGameManager(new GameId("Game Id"));
        }

        [Fact]
        public void LoadPersonalBest_Sets_GameName_Using_Passed_In_Builder()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);

            facade.LoadGameManager(new GameId("Game Id")).GameName.Returns("Game Name");
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Game Name", model.GameName);
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_GameName()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("GameName"));
        }

        [Fact]
        public void LoadPersonalBest_Sets_SplitsName_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Splits Name", model.SplitsName);
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_SplitsName()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("SplitsName"));
        }

        [Fact]
        public void LoadPersonalBets_Sets_RecordingSplits_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            var builder = GetDefaultSplitsBuilder(1);
            var expectedSegments = builder.Segments;
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            Assert.Equal(expectedSegments, model.RecordingSplits);
        }

        [Fact]
        public void FavoriteSplits_Returns_List_From_GameManager()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            var expectedFavoriteSplits = new List<IFileHandler<ISplits>>();

            facade.LoadGameManager(new GameId("Game Id")).FavoriteSplits.Returns(expectedFavoriteSplits);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(expectedFavoriteSplits, model.FavoriteSplits());
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling_And_No_Builder_Is_Loaded()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            model.StopScorePoller();
            Assert.Equal(-1, model.CurrentScore);
        }

        [Fact]
        public void Get_CurrentScore_Returns_PersonalBestBuilder_Score_If_Game_Is_Not_Polling()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            var builder = GetDefaultSplitsBuilder(1);
            builder.GetOutput().EndingSegment.Score.Returns(12345);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            model.StopScorePoller();
            Assert.Equal(12345, model.CurrentScore);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_IsNewPersonalBest_Returns_Value_From_PersonalBestBuilder(bool isNewPb)
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_True()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Assert.Equal(true, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Throws_Exception_If_Game_Is_Not_Running()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);

            facade.LoadGameManager(Arg.Any<GameId>()).GameIsRunning().Returns(false);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Throws<InvalidOperationException>(() => model.StartScorePoller());
        }

        [Fact]
        public void StartScorePoller_Resets_Builder_If_Polling_Is_Not_Already_Started()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var builderMock = GetDefaultSplitsBuilder(1);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            builderMock.Received().Reset();
        }

        [Fact]
        public void StartScorePoller_Does_Not_Reset_Builder_If_Polling_Is_Already_Started()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StartScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_False()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Causes_Loaded_Builder_To_Continuously_Set_CurrentScore()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);

            var builderMock = GetDefaultSplitsBuilder(1);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            facade.LoadGameManager(new GameId("Game Id")).GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.Received().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StartScorePoller_Starts_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(2));

            model.StartScorePoller();
            model.SplitToNextSegment();
            model.SplitToNextSegment();
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StopScorePoller_Fires_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Not_Already_Stopped()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);

            var builderMock = GetDefaultSplitsBuilder(1);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builderMock);
            model.StartScorePoller();
            model.StopScorePoller();
            facade.LoadGameManager(new GameId("Game Id")).GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.DidNotReceive().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StopScorePoller_Fires_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            Thread.Sleep(500);
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Assert.False(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void StopScorePoller_Stops_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_IsNewPersonalBest()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            Thread.Sleep(500);
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Assert.False(eventCatcher.CaughtProperties.Contains("IsNewPersonalBest"));
        }

        [Fact]
        public void StopScorePoller_Returns_Output_Of_Builder_If_Polling_Is_Stopped()
        {
            var model = new PersonalBestTracker(DefaultSplitsFacade());
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
            var model = new PersonalBestTracker(DefaultSplitsFacade());
            var builder = GetDefaultSplitsBuilder(1);
            var expectedSplits = Substitute.For<ISplits>();
            builder.GetOutput().Returns(expectedSplits);

            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", builder);
            Assert.Equal(expectedSplits, model.StopScorePoller());
        }

        [Fact]
        public void Polling_Is_Stopped_If_GameManager_Hook_Becomes_Unhooked()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            facade.LoadGameManager(Arg.Any<GameId>()).GameIsRunning().Returns(false);
            Thread.Sleep(500);
            Assert.False(model.IsPolling);
        }

        [Fact]
        public void Polling_Is_Stopped_If_GameManager_GetCurrentScore_Throws_Error_While_Polling()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            Assert.False(model.IsPolling);
        }

        [Fact]
        public void HasError_Returns_True_If_GameManager_GetCurrentScore_Throws_Error_While_Polling()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));

            model.StartScorePoller();
            Thread.Sleep(500);
            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());

            Thread.Sleep(500);
            Assert.True(model.HasError);
        }

        [Fact]
        public void Fires_NotifyPropertyChanged_Event_For_HasError_When_GameManager_GetCurrentScore_Throws_Error_While_Polling()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            Assert.True(eventCatcher.CaughtProperties.Contains("HasError"));
        }

        [Fact]
        public void LastError_Returns_Exception_Thrown_From_GameManager_GetCurrentScore_While_Polling()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            Exception expectedException = new Exception("Some error message");
            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(expectedException);
            Thread.Sleep(500);
            Assert.Equal(expectedException.Message, model.LastError.Message);
        }

        [Fact]
        public void Fires_NotifyPropertyChanged_Event_For_LastError_When_GameManager_GetCurrentScore_Throws_Error_While_Polling()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            Assert.True(eventCatcher.CaughtProperties.Contains("LastError"));
        }

        [Fact]
        public void ClearError_Sets_LastError_To_Null()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            model.ClearError();
            Assert.Null(model.LastError);
        }

        [Fact]
        public void ClearError_Fires_NotifyPropertyChangedEvent_For_LastError()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            model.ClearError();
            Assert.True(eventCatcher.CaughtProperties.Contains("LastError"));
        }

        [Fact]
        public void ClearError_Sets_HasError_To_False()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            model.ClearError();
            Assert.False(model.HasError);
        }

        [Fact]
        public void ClearError_Fires_NotifyPropertyChangedEvent_For_HasError()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Thread.Sleep(500);

            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            facade.LoadGameManager(Arg.Any<GameId>())
                .When(n => n.GetCurrentScore())
                .Throw(new Exception());
            Thread.Sleep(500);
            model.ClearError();
            Assert.True(eventCatcher.CaughtProperties.Contains("HasError"));
        }

        /// <summary>
        /// In some games (for example Touhou 6), after finishing a run and returning to the main menu, the score in memory will
        /// still read your score from the prevous run. This test and the next test is to check that the PersonalBestTracker
        /// can detect this and returns an initial score of 0 even when this occurs.
        /// </summary>
        [Fact]
        public void CurrentScore_Is_Zero_After_StartPolling_Is_Called_Even_If_GameManager_Is_Greater_Than_Zero()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));

            facade.LoadGameManager(Arg.Any<GameId>()).GetCurrentScore().Returns(12345);
            model.StartScorePoller();
            Thread.Sleep(500);
            Assert.Equal(0, model.CurrentScore);
        }

        [Fact]
        public void CurrentScore_Is_Invokes_GameManager_CurrentScore_After_StartPolling_Is_Called_And_Score_Changes_From_Initial_Value()
        {
            var facade = DefaultSplitsFacade();
            var model = new PersonalBestTracker(facade);
            model.LoadPersonalBest(new GameId("Game Id"), "Splits Name", GetDefaultSplitsBuilder(1));

            facade.LoadGameManager(Arg.Any<GameId>()).GetCurrentScore().Returns(12345);
            model.StartScorePoller();
            Thread.Sleep(500);
            facade.LoadGameManager(Arg.Any<GameId>()).GetCurrentScore().Returns(67890);
            Thread.Sleep(500);
            Assert.Equal(67890, model.CurrentScore);
        }
    }
}
