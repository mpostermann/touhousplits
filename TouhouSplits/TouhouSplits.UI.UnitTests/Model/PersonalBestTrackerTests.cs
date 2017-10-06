﻿using NSubstitute;
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
            return builder;
        }

        [Fact]
        public void LoadPersonalBest_Fires_NotifyPropertyChanged_Event_For_RecordingSplits()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.True(eventCatcher.CaughtProperties.Contains("RecordingSplits"));
        }

        [Fact]
        public void LoadPersonalBest_Loads_Matching_GameManager_If_Manager_Is_Not_Already_Loaded()
        {
            var facadeMock = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facadeMock);

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            facadeMock.Received().LoadGameManager("Game Name");
        }

        [Fact]
        public void LoadPersonalBest_Sets_GameName_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Game Name", model.GameName);
        }

        [Fact]
        public void LoadPersonalBest_Sets_SplitsName_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal("Splits Name", model.SplitsName);
        }

        [Fact]
        public void LoadPersonalBets_Sets_RecordingSplits_Using_Passed_In_Builder()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            var builder = GetDefaultSplitsBuilder(1);
            var expectedSegments = builder.Segments;
            model.LoadPersonalBest("Game Name", "Splits Name", builder);
            Assert.Equal(expectedSegments, model.RecordingSplits);
        }

        [Fact]
        public void FavoriteSplits_Returns_List_From_GameManager()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);
            var expectedFavoriteSplits = new List<IFileHandler<ISplits>>();

            facade.LoadGameManager("Game Name").FavoriteSplits.Returns(expectedFavoriteSplits);
            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(expectedFavoriteSplits, model.FavoriteSplits());
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.StopScorePoller();
            Assert.Equal(-1, model.CurrentScore);
        }

        [Fact]
        public void IsPolling_Is_False_After_Construction()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_True()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            Assert.Equal(true, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Resets_Builder_If_Polling_Is_Not_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(1);

            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);
            model.StartScorePoller();
            builderMock.Received().Reset();
        }

        [Fact]
        public void StartScorePoller_Does_Not_Reset_Builder_If_Polling_Is_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(1);

            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);
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

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StartScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StartScorePoller_Does_Not_Fire_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Already_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StartScorePoller();
            Assert.False(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_False()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
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
            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);
            model.StartScorePoller();
            facade.LoadGameManager("Game Name").Hook.GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.Received().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StartScorePoller_Starts_Continuous_Firing_Of_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.True(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void SplitToNextSegment_Fires_Method_In_Builder_If_Polling_Is_Started()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(2);
            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);

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
            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);

            model.StopScorePoller();
            builderMock.ClearReceivedCalls();
            model.SplitToNextSegment();
            builderMock.DidNotReceive().SplitToNextSegment();
        }

        [Fact]
        public void SplitToNextSegment_Calls_StopPolling_If_Called_At_Last_Segment()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builderMock = GetDefaultSplitsBuilder(2);
            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);

            model.StartScorePoller();
            model.SplitToNextSegment();
            model.ClearReceivedCalls();
            model.SplitToNextSegment();
            model.Received().StopScorePoller();
        }

        [Fact]
        public void StopScorePoller_Fires_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Not_Already_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StopScorePoller();
            Assert.True(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StopScorePoller_Does_Not_Fire_NotifyPropertyChangedEvent_For_IsPolling_If_Polling_Is_Already_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            model.StopScorePoller();
            Assert.False(eventCatcher.CaughtProperties.Contains("IsPolling"));
        }

        [Fact]
        public void StopScorePoller_Causes_Loaded_Builder_To_Stop_Setting_CurrentScore()
        {
            var facade = Substitute.For<ISplitsFacade>();
            var model = new PersonalBestTracker(facade);

            var builderMock = GetDefaultSplitsBuilder(1);
            model.LoadPersonalBest("Game Name", "Splits Name", builderMock);
            model.StartScorePoller();
            model.StopScorePoller();
            facade.LoadGameManager("Game Name").Hook.GetCurrentScore().Returns(12345);
            Thread.Sleep(500);
            builderMock.DidNotReceive().SetScoreForCurrentSegment(12345);
        }

        [Fact]
        public void StopScorePoller_Stops_Firing_Of_NotifyPropertyChangedEvent_For_CurrentScore()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();

            model.LoadPersonalBest("Game Name", "Splits Name", GetDefaultSplitsBuilder(1));
            model.StartScorePoller();
            model.StopScorePoller();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;
            Thread.Sleep(500);
            Assert.False(eventCatcher.CaughtProperties.Contains("CurrentScore"));
        }

        [Fact]
        public void StopScorePoller_Returns_Output_Of_Builder_If_Polling_Is_Stopped()
        {
            var model = new PersonalBestTracker(Substitute.For<ISplitsFacade>());
            var builder = GetDefaultSplitsBuilder(1);
            var expectedSplits = Substitute.For<ISplits>();
            builder.GetOutput().Returns(expectedSplits);

            model.LoadPersonalBest("Game Name", "Splits Name", builder);
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

            model.LoadPersonalBest("Game Name", "Splits Name", builder);
            Assert.Equal(expectedSplits, model.StopScorePoller());
        }
    }
}