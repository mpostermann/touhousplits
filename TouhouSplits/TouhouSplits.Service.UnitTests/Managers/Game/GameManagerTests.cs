using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static IGameConfig CreateConfig(string gameName)
        {
            var config = Substitute.For<IGameConfig>();
            config.RecentSplitsList.Returns(new FileInfo("somefile.ext"));
            config.GameName.Returns(gameName);
            return config;
        }

        private IFileSerializer<List<string>> CreateRecentSplitsSerializer(string filename, params string[] paths)
        {
            var serializer = Substitute.For<IFileSerializer<List<string>>>();
            serializer.Deserialize(filename).Returns(new List<string>(paths));
            return serializer;
        }

        [Fact]
        public void GameName_Returns_GameName_From_Config()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            Assert.Equal("Some game name", manager.GameName);
        }

        [Fact]
        public void RecentSplits_Returns_Splits_Loaded_From_Serializer()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(
                config.RecentSplitsList.FullName,
                "path0",
                "path1",
                "path2"
            );
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            Assert.Equal(new FileInfo("path0").FullName, manager.RecentSplits[0].FileInfo.FullName);
            Assert.Equal(new FileInfo("path1").FullName, manager.RecentSplits[1].FileInfo.FullName);
            Assert.Equal(new FileInfo("path2").FullName, manager.RecentSplits[2].FileInfo.FullName);
        }
    }
}
