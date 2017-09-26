using NSubstitute;
using System.IO;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Serialization;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SplitsFileTests
    {
        [Fact]
        public void FileInfo_Has_FullName_Matching_Path_Set_In_Constructor()
        {
            string path = string.Format("directory/file.{0}", FilePaths.EXT_SPLITS_FILE);
            var file = new SplitsFile(path, Substitute.For<IFileSerializer<ISplits>>());

            FileInfo expectedFileInfo = new FileInfo(path);
            Assert.Equal(expectedFileInfo.FullName, file.FileInfo.FullName);
        }

        [Fact]
        public void Get_Splits_First_Use_Invokes_Deserializer_If_Constructed_Without_A_Splits_Instance()
        {
            FileInfo path = new FileInfo("somepath.someExtension");
            var serializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var file = new SplitsFile(path.FullName, serializerMock);

            var splits = file.Splits;
            serializerMock.Received().Deserialize(path.FullName);
        }

        [Fact]
        public void Get_Splits_Subsequent_Use_Does_Not_Invoke_Deserializer_If_Constructed_Without_A_Splits_Instance()
        {
            FileInfo path = new FileInfo("somepath.someExtension");
            var serializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var file = new SplitsFile(path.FullName, serializerMock);

            var splits0 = file.Splits;
            var splits1 = file.Splits;
            serializerMock.Received(1).Deserialize(path.FullName);
        }

        [Fact]
        public void Get_Splits_Returns_Instance_Passed_In_Constructor()
        {
            var splits = Substitute.For<ISplits>();
            var file = new SplitsFile("somepath.someExtension", splits);

            Assert.Equal(splits, file.Splits);
        }
    }
}
