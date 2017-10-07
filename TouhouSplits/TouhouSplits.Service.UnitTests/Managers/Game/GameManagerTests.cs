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
        private static IGameConfig CreateConfig(string gameId, string gameName)
        {
            var config = Substitute.For<IGameConfig>();
            config.FavoriteSplitsList.Returns(new FileInfo("somefile.ext"));
            config.Id.Returns(new GameId(gameId));
            config.GameName.Returns(gameName);
            return config;
        }

        private IFileSerializer<List<string>> CreateFavoriteSplitsSerializer(FileInfo filename, params string[] paths)
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

        private IFileHandler<ISplits> CreateSplitsFile(string filepath, string gameId)
        {
            var splitsFile = Substitute.For<IFileHandler<ISplits>>();
            splitsFile.FileInfo.Returns(new FileInfo(filepath));
            splitsFile.Object.GameId.Returns(new GameId(gameId));
            return splitsFile;
        }

        [Fact]
        public void Constructor_Invokes_FavoriteSplitsSerializer_If_File_Does_Not_Exist()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializerMock = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            favoriteSplitsSerializerMock
                .Deserialize(config.FavoriteSplitsList)
                .Returns(n => { throw new FileNotFoundException(); });

            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );
            favoriteSplitsSerializerMock.Received().Serialize(
                Arg.Is<List<string>>(n => n.Count == 0),
                config.FavoriteSplitsList
            );
        }

        [Fact]
        public void Constructor_Invokes_FavoriteSplitsSerializer_If_Directory_Does_Not_Exist()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializerMock = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            favoriteSplitsSerializerMock
                .Deserialize(config.FavoriteSplitsList)
                .Returns(n => { throw new DirectoryNotFoundException(); });

            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );
            favoriteSplitsSerializerMock.Received().Serialize(
                Arg.Is<List<string>>(n => n.Count == 0),
                config.FavoriteSplitsList
            );
        }

        [Fact]
        public void Id_Returns_Id_From_Config()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.Equal(new GameId("Some id"), manager.Id);
        }

        [Fact]
        public void GameName_Returns_GameName_From_Config()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.Equal("Some game name", manager.GameName);
        }

        [Fact]
        public void FavoriteSplits_Returns_Splits_Filepaths_Loaded_From_FavoriteSplitsSerializer()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(
                config.FavoriteSplitsList,
                "path0",
                "path1",
                "path2"
            );
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.Equal(new FileInfo("path0").FullName, manager.FavoriteSplits[0].FileInfo.FullName);
            Assert.Equal(new FileInfo("path1").FullName, manager.FavoriteSplits[1].FileInfo.FullName);
            Assert.Equal(new FileInfo("path2").FullName, manager.FavoriteSplits[2].FileInfo.FullName);
        }

        [Fact]
        public void FavoriteSplits_Deserialize_Splits_Using_Passed_In_Serializer()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(
                config.FavoriteSplitsList,
                "path0"
            );
            var splitsSerializerMock = Substitute.For<IFileSerializer<Splits>>();
            splitsSerializerMock.Deserialize(Arg.Any<FileInfo>()).Returns(Substitute.For<Splits>());
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                splitsSerializerMock
            );

            var splits = manager.FavoriteSplits[0].Object;
            splitsSerializerMock.Received().Deserialize(Arg.Is<FileInfo>(n => n.Name == "path0"));
        }

        [Fact]
        public void FavoriteSplits_Item_Is_Updated_If_AddOrUpdateFavorites_Is_Called_For_An_Existing_File()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList, "path0");
            var splitsSerializer = Substitute.For<IFileSerializer<Splits>>();
            splitsSerializer.Deserialize(Arg.Is<FileInfo>(n => n.Name == "path0")).Returns(Substitute.For<Splits>());
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                splitsSerializer
            );

            var updatedSplitsFile = CreateSplitsFile("path0", "Some id");
            var expectedSplits = updatedSplitsFile.Object;

            manager.AddOrUpdateFavorites(updatedSplitsFile);
            Assert.Equal(expectedSplits, manager.FavoriteSplits.FirstOrDefault(n => n.FileInfo.Name == "path0").Object);
        }

        [Fact]
        public void AddOrUpdateFavorites_Invokes_FavoriteSplitsSerializer_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializerMock = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateFavorites(CreateSplitsFile("new splits path", "Some id"));

            var splitsPaths = favoriteSplitsSerializerMock.Deserialize(config.FavoriteSplitsList);
            Assert.True(splitsPaths.Contains(new FileInfo("new splits path").FullName));
            favoriteSplitsSerializerMock.Received().Serialize(splitsPaths, config.FavoriteSplitsList);
        }

        [Fact]
        public void AddOrUpdateFavorites_Does_Not_Invokes_FavoriteSplitsSerializer_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializerMock = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializerMock,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateFavorites(CreateSplitsFile("existing splits path", "Some id"));

            var splitsPaths = favoriteSplitsSerializerMock.Deserialize(config.FavoriteSplitsList);
            Assert.Equal(1, splitsPaths.Count);
            favoriteSplitsSerializerMock.DidNotReceive().Serialize(splitsPaths, config.FavoriteSplitsList);
        }

        [Fact]
        public void AddOrUpdateFavorites_Adds_Splits_To_FavoriteSplits_If_Filepath_Is_Not_Already_In_List()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateFavorites(CreateSplitsFile("new splits path", "Some id"));

            Assert.Equal(1, manager.FavoriteSplits.Count);
            Assert.Equal(new FileInfo("new splits path").FullName, manager.FavoriteSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void AddOrUpdateFavorites_Does_Not_Add_Splits_To_FavoriteSplits_If_Filepath_Is_Already_In_List()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList, "existing splits path");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            manager.AddOrUpdateFavorites(CreateSplitsFile("existing splits path", "Some id"));

            Assert.Equal(1, manager.FavoriteSplits.Count);
            Assert.Equal(new FileInfo("existing splits path").FullName, manager.FavoriteSplits[0].FileInfo.FullName);
        }

        [Fact]
        public void AddOrUpdateFavorites_Throws_Exception_If_Splits_GameId_Does_Not_Equal_GameManagers_GameId()
        {
            var config = CreateConfig("Some id", "Some game name");
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>()
            );

            var splitsFile = CreateSplitsFile("some path", "Not matching game id");
            Assert.Throws<InvalidOperationException>(() => manager.AddOrUpdateFavorites(splitsFile));
        }
    }
}
