using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private IFileSerializer<List<string>> CreateRecentSplitsSerializer(FileInfo filename, params string[] paths)
        {
            var serializer = Substitute.For<IFileSerializer<List<string>>>();
            serializer
                .Deserialize(filename)
                .Returns(new List<string>());
            foreach (string path in paths) {
                serializer.Deserialize(filename).Add(new FileInfo(path).FullName);
            }
            return serializer;
        }

        private ISplitsFile CreateSplitsFile(string filepath)
        {
            var splitsFile = Substitute.For<ISplitsFile>();
            splitsFile.FileInfo.Returns(new FileInfo(filepath));
            return splitsFile;
        }

        [Fact]
        public void Constructor_Invokes_RecentSplitsSerializer_If_File_Does_Not_Exist()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList);
            recentSplitsSerializerMock
                .Deserialize(config.RecentSplitsList)
                .Returns(n => { throw new FileNotFoundException(); });

            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );
            recentSplitsSerializerMock.Received().Serialize(
                Arg.Is<List<string>>(n => n.Count == 0),
                config.RecentSplitsList
            );
        }

        [Fact]
        public void Constructor_Invokes_RecentSplitsSerializer_If_Directory_Does_Not_Exist()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList);
            recentSplitsSerializerMock
                .Deserialize(config.RecentSplitsList)
                .Returns(n => { throw new DirectoryNotFoundException(); });

            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );
            recentSplitsSerializerMock.Received().Serialize(
                Arg.Is<List<string>>(n => n.Count == 0),
                config.RecentSplitsList
            );
        }

        [Fact]
        public void GameName_Returns_GameName_From_Config()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.Equal("Some game name", manager.GameName);
        }

        [Fact]
        public void RecentSplits_Returns_Splits_Filepaths_Loaded_From_RecentSplitsSerializer()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(
                config.RecentSplitsList,
                "path0",
                "path1",
                "path2"
            );
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
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
                config.RecentSplitsList,
                "path0"
            );
            var splitsSerializerMock = Substitute.For<IFileSerializer<Splits>>();
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializerMock
            );

            var splits = manager.RecentSplits[0].Splits;
            splitsSerializerMock.Received().Deserialize(Arg.Is<FileInfo>(n => n.Name == "path0"));
        }

        [Fact]
        public void RecentSplits_Item_Is_Updated_If_AddOrUpdateRecentSplits_Is_Called_For_An_Existing_File()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList, "path0");
            var splitsSerializer = Substitute.For<IFileSerializer<Splits>>();
            splitsSerializer.Deserialize(Arg.Is<FileInfo>(n => n.Name == "path0")).Returns(Substitute.For<Splits>());
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                splitsSerializer
            );

            var updatedSplitsFile = CreateSplitsFile("path0");
            var expectedSplits = updatedSplitsFile.Splits;

            manager.AddOrUpdateRecentSplits(updatedSplitsFile);
            Assert.Equal(expectedSplits, manager.RecentSplits.FirstOrDefault(n => n.FileInfo.Name == "path0").Splits);
        }

        [Fact]
        public void AddOrUpdateRecentSplits_Invokes_RecentSplitsSerializer_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateRecentSplits(CreateSplitsFile("new splits path"));

            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList);
            Assert.True(splitsPaths.Contains(new FileInfo("new splits path").FullName));
            recentSplitsSerializerMock.Received().Serialize(splitsPaths, config.RecentSplitsList);
        }

        [Fact]
        public void AddOrUpdateRecentSplits_Does_Not_Invokes_RecentSplitsSerializer_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializerMock = CreateRecentSplitsSerializer(config.RecentSplitsList, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateRecentSplits(CreateSplitsFile("existing splits path"));

            var splitsPaths = recentSplitsSerializerMock.Deserialize(config.RecentSplitsList);
            Assert.Equal(1, splitsPaths.Count);
            recentSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.RecentSplitsList);
        }

        [Fact]
        public void AddOrUpdateRecentSplits_Adds_Splits_To_RecentSplits_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateRecentSplits(CreateSplitsFile("new splits path"));

            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("new splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void AddOrUpdateRecentSplits_Does_Not_Add_Splits_To_RecentSplits_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some game name");
            var recentSplitsSerializer = CreateRecentSplitsSerializer(config.RecentSplitsList, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                recentSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateRecentSplits(CreateSplitsFile("existing splits path"));

            Assert.Equal(1, manager.RecentSplits.Count);
            Assert.Equal(new FileInfo("existing splits path").FullName, manager.RecentSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void AddOrUpdateRecentSplits_Throws_Exception_If_Splits_GameName_Does_Not_Equal_GameManagers_GameName()
        {
            var config = CreateConfig("Some game name");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                CreateRecentSplitsSerializer(config.RecentSplitsList),
                Substitute.For<IFileSerializer<Splits>>()
            );

            var splitsFile = CreateSplitsFile("some path");
            splitsFile.Splits.GameName.Returns("Some other game name");
            Assert.Throws<InvalidOperationException>(() => manager.AddOrUpdateRecentSplits(splitsFile));
        }
    }
}
