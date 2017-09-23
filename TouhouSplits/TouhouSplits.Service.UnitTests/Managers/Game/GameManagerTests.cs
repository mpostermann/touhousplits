using NSubstitute;
using System.Collections.Generic;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Serialization;
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

            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                Substitute.For<IFileSerializer<List<SplitsFile>>>()
            );
            Assert.Equal("Some game name", manager.GameName);
        }
    }
}
