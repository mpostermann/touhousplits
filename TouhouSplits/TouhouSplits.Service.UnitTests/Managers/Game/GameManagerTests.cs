﻿using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Hook.Impl;
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
        public void FavoriteSplitsFileLoaded_Returns_True_If_FavoriteSplits_Was_Deserialized_Without_Error()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.True(manager.FavoriteSplitsFileLoaded);
        }

        [Fact]
        public void FavoriteSplitsFileLoaded_Returns_False_If_FavoriteSplits_Was_Deserialized_With_Errors()
        {
            var config = CreateConfig("Some id", "Some game name");
            var favoriteSplitsSerializer = CreateFavoriteSplitsSerializer(config.FavoriteSplitsList);
            favoriteSplitsSerializer.Deserialize(Arg.Any<FileInfo>()).Returns(n => { throw new Exception(); });
            var manager = new GameManager(
                config,
                Substitute.For<IHookStrategyFactory>(),
                favoriteSplitsSerializer,
                Substitute.For<IFileSerializer<Splits>>()
            );

            Assert.False(manager.FavoriteSplitsFileLoaded);
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

        [Fact]
        public void GameIsHookable_Returns_True_If_Hook_Is_NonHookingStrategy()
        {
            // Setup the strategy factory to create a NonHookingStrategy
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();
            factory.Create(config.HookConfig).Returns(new NonHookingStrategy());

            // GameIsHookable should be false since we're using a NonHookingStrategy
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            Assert.Equal(false, manager.GameIsHookable);
        }

        [Fact]
        public void GameIsHookable_Returns_False_If_Hook_Is_Not_NonHookingStrategy()
        {
            // Setup the strategy factory as normal
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();

            // GameIsHookable should be true since we're using a strategy that is not a NonHookingStrategy
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            Assert.Equal(true, manager.GameIsHookable);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GameIsRunning_Returns_Hook_GameIsRunning_Method(bool isRunning)
        {
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            factory.Create(config.HookConfig).GameIsRunning().Returns(isRunning);
            Assert.Equal(isRunning, manager.GameIsRunning());
        }

        [Fact]
        public void GetCurrentScore_Invokes_Hooks_Hook_Method_If_Game_Is_Not_Already_Hooked()
        {
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            var hook = factory.Create(config.HookConfig);
            hook.GameIsRunning().Returns(true);
            hook.IsHooked.Returns(false);

            manager.GetCurrentScore();
            hook.Received().Hook();
        }

        [Fact]
        public void GetCurrentScore_Does_Not_Invoke_Hooks_Hook_Method_If_Game_Is_Already_Hooked()
        {
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            var hook = factory.Create(config.HookConfig);
            hook.GameIsRunning().Returns(true);
            hook.IsHooked.Returns(true);

            manager.GetCurrentScore();
            hook.DidNotReceive().Hook();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(23409823)]
        public void GetCurrentScore_Returns_Hook_GetCurrentScore_Method(long score)
        {
            var config = CreateConfig("Some id", "Some game name");
            var factory = Substitute.For<IHookStrategyFactory>();
            var manager = new GameManager(
                config,
                factory,
                CreateFavoriteSplitsSerializer(config.FavoriteSplitsList),
                Substitute.For<IFileSerializer<Splits>>());

            var hook = factory.Create(config.HookConfig);
            hook.GameIsRunning().Returns(true);
            hook.GetCurrentScore().Returns(score);

            Assert.Equal(score, manager.GetCurrentScore());
        }
    }
}
