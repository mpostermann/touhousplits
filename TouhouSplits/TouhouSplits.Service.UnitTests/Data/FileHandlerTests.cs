using NSubstitute;
using System;
using System.IO;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Serialization;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class FileHandlerTests
    {
        [Fact]
        public void Constructor_With_Obj_Sets_Object_To_Obj()
        {
            var obj = Substitute.For<Splits>();
            var fileHandler = new FileHandler<ISplits, Splits>(obj, Substitute.For<IFileSerializer<Splits>>());
            Assert.Equal(obj, fileHandler.Object);
        }

        [Fact]
        public void Object_Invokes_Serializer_If_Object_Is_Not_Loaded()
        {
            var serializer = Substitute.For<IFileSerializer<Splits>>();
            var fileHandler = new FileHandler<ISplits, Splits>(new FileInfo("some path"), serializer);

            var obj = fileHandler.Object;
            serializer.Received(1).Deserialize(Arg.Is<FileInfo>(n => n.Name == "some path"));
        }

        [Fact]
        public void Object_Does_Not_Invoke_Serializer_After_Object_Is_Loaded()
        {
            var serializer = Substitute.For<IFileSerializer<Splits>>();
            var fileHandler = new FileHandler<ISplits, Splits>(new FileInfo("some path"), serializer);

            var obj = fileHandler.Object;
            obj = fileHandler.Object;
            serializer.Received(1).Deserialize(Arg.Is<FileInfo>(n => n.Name == "some path"));
        }

        [Fact]
        public void Object_Invokes_Serializer_After_Close_Is_Called()
        {
            var serializer = Substitute.For<IFileSerializer<Splits>>();
            var fileHandler = new FileHandler<ISplits, Splits>(new FileInfo("some path"), serializer);

            var obj = fileHandler.Object;
            fileHandler.Close();
            serializer.ClearReceivedCalls();
            obj = fileHandler.Object;
            serializer.Received(1).Deserialize(Arg.Is<FileInfo>(n => n.Name == "some path"));
        }

        [Fact]
        public void Object_Throws_Exception_If_Object_And_FileInfo_Are_Null()
        {
            var fileHandler = new FileHandler<ISplits, Splits>(
                Substitute.For<Splits>(),
                Substitute.For<IFileSerializer<Splits>>());

            Assert.Throws<InvalidOperationException>(() => fileHandler.Object);
        }

        [Fact]
        public void Object_Is_Cloned_When_Constructed_With_Obj()
        {
            var splits = Substitute.For<Splits>();
            var clone = Substitute.For<Splits>();
            splits.Clone().Returns(clone);
            var fileHandler = new FileHandler<ISplits, Splits>(splits, Substitute.For<IFileSerializer<Splits>>());

            clone.GameName = "Original Game Name";
            fileHandler.Object.GameName = "New Game Name";
            fileHandler.RevertToLastSavedState();
            Assert.Equal("Original Game Name", fileHandler.Object.GameName);
        }

        [Fact]
        public void Object_Is_Cloned_When_Deserialized()
        {
            var splits = Substitute.For<Splits>();
            var clone = Substitute.For<Splits>();
            splits.Clone().Returns(clone);
            var serializer = Substitute.For<IFileSerializer<Splits>>();
            var fileHandler = new FileHandler<ISplits, Splits>(new FileInfo("some path"), serializer);

            serializer.Deserialize(Arg.Is<FileInfo>(n => n.Name == "some path")).Returns(splits);
            clone.GameName = "Original Game Name";
            fileHandler.Object.GameName = "New Game Name";
            fileHandler.RevertToLastSavedState();
            Assert.Equal("Original Game Name", fileHandler.Object.GameName);
        }

        [Fact]
        public void Object_Is_Cloned_When_RevertToLastSavedState_Is_Invoked()
        {
            var splits = Substitute.For<Splits>();
            var clone = Substitute.For<Splits>();
            var clonedClone = Substitute.For<Splits>();
            splits.Clone().Returns(clone);
            clone.Clone().Returns(clonedClone);
            var fileHandler = new FileHandler<ISplits, Splits>(splits, Substitute.For<IFileSerializer<Splits>>());

            clonedClone.GameName = "Original Game Name";
            fileHandler.RevertToLastSavedState();
            fileHandler.Object.GameName = "New Game Name";
            fileHandler.RevertToLastSavedState();
            Assert.Equal("Original Game Name", fileHandler.Object.GameName);
        }

        [Fact]
        public void Object_Is_Cloned_When_Save_Is_Invoked()
        {
            var splits = Substitute.For<Splits>();
            var clone = Substitute.For<Splits>();
            var clonedClone = Substitute.For<Splits>();
            splits.Clone().Returns(clone);
            clone.Clone().Returns(clonedClone);
            var fileHandler = new FileHandler<ISplits, Splits>(splits, Substitute.For<IFileSerializer<Splits>>());

            clonedClone.GameName = "Updated Game Name";
            fileHandler.Save();
            fileHandler.Object.GameName = "Newest Game Name";
            fileHandler.RevertToLastSavedState();
            Assert.Equal("Updated Game Name", fileHandler.Object.GameName);
        }

        [Fact]
        public void Save_Invokes_Serializer()
        {
            var serializerMock = Substitute.For<IFileSerializer<Splits>>();
            var fileHandler = new FileHandler<ISplits, Splits>(new FileInfo("some path"), serializerMock);

            fileHandler.Save();
            serializerMock.Received().Serialize(Arg.Any<Splits>(), Arg.Is<FileInfo>(n => n.Name == "some path"));
        }

        [Fact]
        public void Save_Throws_Exception_If_FileInfo_Is_Null()
        {
            var fileHandler = new FileHandler<ISplits, Splits>(
                            Substitute.For<Splits>(),
                            Substitute.For<IFileSerializer<Splits>>());

            Assert.Throws<InvalidOperationException>(() => fileHandler.Save());
        }
    }
}
