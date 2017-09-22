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
    public class GameTests
    {
        [Fact]
        public void Set_GameName_Fires_NotifyPropertyChanged_Event_For_GameManager()
        {
            var game = new Game(
                Substitute.For<ISplitsFacade>(),
                Substitute.For<IGameManager>()
            );
            var eventCatcher = new NotifyPropertyChangedCatcher();
            game.PropertyChanged += eventCatcher.CatchPropertyChangedEvents;

            game.GameName = "Some game name";
            Assert.True(eventCatcher.CaughtProperties.Contains("GameManager"));
        }

        [Fact]
        public void Set_GameName_Fires_NotifyPropertyChanged_Event_For_RecentSplits()
        {
            var game = new Game(
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
            throw new NotImplementedException();
        }

        [Fact]
        public void Get_CurrentScore_Returns_Negative_One_If_Game_Is_Not_Polling()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void IsPolling_Is_False_After_Construction()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void StartScorePoller_Sets_IsPoller_To_True()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void StartScorePoller_Sets_IsPoller_To_False()
        {
            throw new NotImplementedException();
        }
    }
}
