﻿using NSubstitute;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Managers.Game;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Managers.Game
{
    public class GameManagerTests
    {
        [Fact]
        public void GameName_Returns_GameName_From_Config()
        {
            var config = Substitute.For<IGameConfig>();
            config.GameName.Returns("Some game name");

            var manager = new GameManager(config);
            Assert.Equal("Some game name", manager.GameName);
        }
    }
}
