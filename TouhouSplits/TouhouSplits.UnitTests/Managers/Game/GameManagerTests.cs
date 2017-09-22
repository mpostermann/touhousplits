using NSubstitute;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Managers.Game;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Managers.Game
{
    public class GameManagerTests
    {
        [Fact]
        public void NameIsCorrect()
        {
            var config = Substitute.For<IGameConfig>();
            config.GameName.Returns("Some game name");

            var manager = new GameManager(config);
            Assert.Equal(config.GameName, manager.GameName);
        }
    }
}
