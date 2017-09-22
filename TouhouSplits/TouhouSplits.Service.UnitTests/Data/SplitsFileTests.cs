using NSubstitute;
using System.IO;
using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Data
{
    public class SplitsFileTests
    {
        [Fact]
        public void FileInfo_Has_FullName_Matching_Path_Set_In_Constructor()
        {
            string path = string.Format("directory/file.{0}", SplitsFacade.DEFAULT_FILE_EXT);
            var file = new SplitsFile(path, null);

            FileInfo expectedFileInfo = new FileInfo(path);
            Assert.Equal(expectedFileInfo.FullName, file.FileInfo.FullName);
        }

        [Fact]
        public void Splits_Returns_Instance_Set_In_Constructor()
        {
            string path = "Somepath.someExtension";
            var splits = Substitute.For<ISplits>();
            var file = new SplitsFile(path, splits);

            Assert.Equal(splits, file.Splits);
        }
    }
}
