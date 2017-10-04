using NSubstitute;
using System.Collections.Generic;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using TouhouSplits.UI.Model;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Model
{
    public class MainModelTests
    {
        private static IFileHandler<ISplits> GetDefaultSplitsFile(int numSegments)
        {
            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFile.Object.Segments.Returns(new List<ISegment>());
            for (int i = 0; i < numSegments; i++) {
                splitsFile.Object.Segments.Add(Substitute.For<ISegment>());
            }
            return splitsFile;
        }

        [Fact]
        public void Set_CurrentSplitsFile_Fires_NotifyPropertyChanged_Event_For_CurrentSplitsFile()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.CurrentSplitsFile = Substitute.For<IFileHandler<ISplits>>();
            Assert.True(eventCatcher.CaughtProperties.Contains("CurrentSplitsFile"));
        }

        [Fact]
        public void Set_CurrentSplitsFile_Fires_NotifyPropertyChanged_Event_For_RecordingSplits()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());
            var eventCatcher = new NotifyPropertyChangedCatcher();
            model.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            model.CurrentSplitsFile = Substitute.For<IFileHandler<ISplits>>();
            Assert.True(eventCatcher.CaughtProperties.Contains("RecordingSplits"));
        }

        [Fact]
        public void Set_CurrentSplitsFile_Loads_Matching_GameManager_If_Manager_Is_Not_Already_Loaded()
        {
            var facadeMock = Substitute.For<ISplitsFacade>();
            var model = new MainModel(facadeMock);

            var splitsFile = GetDefaultSplitsFile(1);
            splitsFile.Object.GameName = "Game to load";
            model.CurrentSplitsFile = splitsFile;
            facadeMock.Received().LoadGameManager("Game to load");
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());

            model.StopScorePoller();
            Assert.Equal(-1, model.CurrentScore);
        }

        [Fact]
        public void IsPolling_Is_False_After_Construction()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());

            model.CurrentSplitsFile = GetDefaultSplitsFile(1);
            Assert.Equal(false, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_True()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());

            model.CurrentSplitsFile = GetDefaultSplitsFile(1);
            model.StartScorePoller();
            Assert.Equal(true, model.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPolling_To_False()
        {
            var model = new MainModel(Substitute.For<ISplitsFacade>());

            model.CurrentSplitsFile = GetDefaultSplitsFile(1);
            model.StartScorePoller();
            model.StopScorePoller();
            Assert.Equal(false, model.IsPolling);
        }
    }
}
