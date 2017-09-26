using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
            serializer.Deserialize(filename).Returns(new List<string>());
            foreach (string path in paths) {
                serializer.Deserialize(filename).Add(new FileInfo(path).FullName);
            }
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

        [Fact]
        public void RecentSplits_Deserialize_Splits_Using_Passed_In_Serializer()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(
                config.RecentSplitsList.FullName,
                "path0"
            );
            var splitsSerializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializerMock
            );

            var splits = manager.RecentSplits[0].Splits;
            splitsSerializerMock.Received().Deserialize(new FileInfo("path0").FullName);
        }

        [Fact]
        public void SerializeSplits_Invokes_SplitsSerializer()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var splitsSerializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializerMock
            );

            var splits = new Splits();
            manager.SerializeSplits(splits, new FileInfo("new splits path"));
            splitsSerializerMock.Received().Serialize(splits, new FileInfo("new splits path").FullName);
        }

        [Fact]
        public void SerializeSplits_Invokes_RecentSplitsSerializer_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.SerializeSplits(new Splits(), new FileInfo("new splits path"));
            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
            Assert.True(splitsPaths.Contains(new FileInfo("new splits path").FullName));
            recentSplitsSerializerMock.Received().Serialize(splitsPaths, config.RecentSplitsList.FullName);
        }

        [Fact]
        public void SerializeSplits_Does_Not_Invokes_RecentSplitsSerializer_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.SerializeSplits(new Splits(), new FileInfo("existing splits path"));
            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
            Assert.Equal(1, splitsPaths.Count);
            recentSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.RecentSplitsList.FullName);
        }

        [Fact]
        public void SerializeSplits_Adds_Splits_To_RecentSplits_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.SerializeSplits(new Splits(), new FileInfo("new splits path"));
            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("new splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void SerializeSplits_Does_Not_Add_Splits_To_RecentSplits_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.SerializeSplits(new Splits(), new FileInfo("existing splits path"));
            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("existing splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void SerializeSplits_Does_Not_Invokes_RecentSplitsSerializer_If_Serialization_Fails()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "good splits path", "bad splits path");
            var splitsSerializer = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                splitsSerializer
            );

            var badSplits = new Splits();
            splitsSerializer
                .When(n => n.Serialize(badSplits, new FileInfo("bad splits path").FullName))
                .Do(n => { throw new Exception(); });
            try {
                manager.SerializeSplits(badSplits, new FileInfo("bad splits path"));
            }
            catch {
                var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
                Assert.True(splitsPaths.Contains(new FileInfo("bad splits path").FullName));
                recentSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.RecentSplitsList.FullName);
            }
        }

        [Fact]
        public void SerializeSplits_Does_Not_Add_To_RecentSplits_If_Serialization_Fails()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "good splits path");
            var splitsSerializer = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializer
            );

            var badSplits = new Splits();
            splitsSerializer
                .When(n => n.Serialize(badSplits, new FileInfo("bad splits path").FullName))
                .Do(n => { throw new Exception(); });
            try {
                manager.SerializeSplits(badSplits, new FileInfo("bad splits path"));
            }
            catch {
                Assert.Equal(1, manager.RecentSplits.Count);
                Assert.Equal(new FileInfo("good splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
            }
        }

        [Fact]
        public void DeserializeSplits_Invokes_SplitsSerializer()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var splitsSerializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializerMock
            );

            manager.DeserializeSplits(new FileInfo("some splits path"));
            splitsSerializerMock.Received().Deserialize(new FileInfo("some splits path").FullName);
        }

        [Fact]
        public void DeserializeSplits_Invokes_RecentSplitsSerializer_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.DeserializeSplits(new FileInfo("some splits path"));
            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
            Assert.True(splitsPaths.Contains(new FileInfo("some splits path").FullName));
            recentSplitsSerializerMock.Received().Serialize(splitsPaths, config.RecentSplitsList.FullName);
        }

        [Fact]
        public void DeserializeSplits_Does_Not_Invokes_RecentSplitsSerializer_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.DeserializeSplits(new FileInfo("existing splits path"));
            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
            Assert.Equal(1, splitsPaths.Count);
            recentSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.RecentSplitsList.FullName);
        }

        [Fact]
        public void DeserializeSplits_Adds_Splits_To_RecentSplits_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.DeserializeSplits(new FileInfo("new splits path"));
            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("new splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void DeserializeSplits_Does_Not_Add_Splits_To_RecentSplits_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<ISplits>>()
            );

            manager.DeserializeSplits(new FileInfo("existing splits path"));
            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("existing splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void DeserializeSplits_Does_Not_Invokes_RecentSplitsSerializer_If_Serialization_Fails()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "good splits path", "bad splits path");
            var splitsSerializer = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                splitsSerializer
            );

            splitsSerializer
                .Deserialize(new FileInfo("bad splits path").FullName)
                .Returns(n => { throw new Exception(); } );
            try {
                manager.DeserializeSplits(new FileInfo("bad splits path"));
            }
            catch {
                var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList.FullName);
                Assert.True(splitsPaths.Contains(new FileInfo("bad splits path").FullName));
                recentSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.RecentSplitsList.FullName);
            }
        }

        [Fact]
        public void DeserializeSplits_Does_Not_Add_To_RecentSplits_If_Serialization_Fails()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList.FullName, "good splits path");
            var splitsSerializer = Substitute.For<IFileSerializer<ISplits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializer
            );

            var badSplits = new Splits();
            splitsSerializer
                .Deserialize(new FileInfo("bad splits path").FullName)
                .Returns(n => { throw new Exception(); });
            try {
                manager.DeserializeSplits(new FileInfo("bad splits path"));
            }
            catch {
                Assert.Equal(1, manager.RecentSplits.Count);
                Assert.Equal(new FileInfo("good splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
            }
        }
    }
}
