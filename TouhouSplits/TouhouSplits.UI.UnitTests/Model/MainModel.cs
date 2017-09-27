using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSplits.Service;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.UI.Model;
using TouhouSplits.UnitTests.Utils;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Model
{
    public class MainModel
    {
        [Fact]
        public void Set_GameName_Fires_NotifyPropertyChanged_Event_For_RecentSplits()
        {
            var game = new UI.Model.MainModel(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );
            var eventCatcher = new NotifyPropertyChangedCatcher();
            game.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            game.GameName = "Some game name";
            Assert.True(eventCatcher.CaughtProperties.Contains("RecentSplits"));
        }

        [Fact]
        public void Set_GameName_Loads_Matching_GameManager()
        {
            var facadeMock = Substitute.For<ISplitsFacade>();
            var game = new UI.Model.MainModel(
                facadeMock,
                Substitute.For<IGameManager>()
            );

            game.GameName = "Game to load";
            facadeMock.Received().LoadGameManager("Game to load");
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling()
        {
            var game = new UI.Model.MainModel(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );

            game.StopScorePoller();
            Assert.Equal(-1, game.CurrentScore);
        }

        [Fact]
        public void IsPolling_Is_False_After_Construction()
        {
            var game = new UI.Model.MainModel(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );

            Assert.Equal(false, game.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPoller_To_True()
        {
            var game = new UI.Model.MainModel(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );

            game.StartScorePoller();
            Assert.Equal(true, game.IsPolling);
        }

        [Fact]
        public void StartScorePoller_Sets_IsPoller_To_False()
        {
            var game = new UI.Model.MainModel(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );

            game.StopScorePoller();
            Assert.Equal(false, game.IsPolling);
        }
    }
}
