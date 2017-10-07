using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Managers.Game;
using TouhouSplits.Service.Serialization;
using Xunit;

namespace TouhouSplits.Service.UnitTests
{
    public class SplitsFacadeTests
    {
        private static IConfigManager DefaultConfigManager(int numGames)
        {
            var manager = Substitute.For<IConfigManager>();
            manager.AvailableGames.Returns(new List<IGameConfig>());
            for (int i = 0; i < numGames; i++) {
                manager.AvailableGames.Add(Substitute.For<IGameConfig>());
                var id = new GameId(string.Format("Id {0}", i));
                manager.AvailableGames[i].Id.Returns(id);
                manager.AvailableGames[i].GameName.Returns(string.Format("Game {0}", i));
            }
            return manager;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AvailableGames_Has_Same_Count_As_Config(int numGames)
        {
            var facade = new SplitsFacade(DefaultConfigManager(numGames), Substitute.For<IFileSerializer<Splits>>());
            Assert.Equal(numGames, facade.AvailableGames.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AvailableGames_Has_Same_Ids_As_Config(int numGames)
        {
            var facade = new SplitsFacade(DefaultConfigManager(numGames), Substitute.For<IFileSerializer<Splits>>());
            for (int i = 0; i < numGames; i++) {
                GameId expectedId = new GameId(string.Format("Id {0}", i));
                Assert.Equal(expectedId, facade.AvailableGames[i]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AvailableGameNames_Has_Same_Count_As_Config(int numGames)
        {
            var facade = new SplitsFacade(DefaultConfigManager(numGames), Substitute.For<IFileSerializer<Splits>>());
            Assert.Equal(numGames, facade.AvailableGameNames.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AvailableGameNames_Has_Same_Names_As_Config(int numGames)
        {
            var facade = new SplitsFacade(DefaultConfigManager(numGames), Substitute.For<IFileSerializer<Splits>>());
            for (int i = 0; i < numGames; i++) {
                string expectedId = string.Format("Game {0}", i);
                Assert.Equal(expectedId, facade.AvailableGameNames[i]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void GetIdFromName_Returns_Id_From_Config_With_Matching_GameName(int numGames)
        {
            var facade = new SplitsFacade(DefaultConfigManager(numGames), Substitute.For<IFileSerializer<Splits>>());
            for (int i = 0; i < numGames; i++) {
                GameId expectedId = new GameId(string.Format("Id {0}", i));
                Assert.Equal(expectedId, facade.GetIdFromName(string.Format("Game {0}", i)));
            }
        }

        [Fact]
        public void GetIdFromName_Throws_Exception_If_Called_Using_Id_Not_In_ConfigManager()
        {
            var facade = new SplitsFacade(DefaultConfigManager(3), Substitute.For<IFileSerializer<Splits>>());
            Assert.Throws<KeyNotFoundException>(() => facade.GetIdFromName("Unknown game name"));
        }
    }
}
