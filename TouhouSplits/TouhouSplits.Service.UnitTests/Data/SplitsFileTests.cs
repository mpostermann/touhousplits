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
        public void Get_Splits_Invokes_Deserializer()
        {
            FileInfo path = new FileInfo("somepath.someExtension");
            var serializerMock = Substitute.For<IFileSerializer<ISplits>>();
            var file = new SplitsFile(path.FullName, serializerMock);

            var splits = file.Splits;
            serializerMock.Received().Deserialize(path.FullName);
        }
    }
}
