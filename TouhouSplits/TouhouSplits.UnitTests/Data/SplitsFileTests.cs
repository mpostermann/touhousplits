using NSubstitute;
using System.IO;
using TouhouSplits.Service;
using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.UnitTests.Data
{
    public class SplitsFileTests
    {
        [Fact]
        public void FileinfoHasCorrectName()
        {
            string path = string.Format("directory/file.{0}", SplitsFacade.DEFAULT_FILE_EXT);
            var file = new SplitsFile(path, null);

            FileInfo expectedFileInfo = new FileInfo(path);
            Assert.Equal(expectedFileInfo.FullName, file.FileInfo.FullName);
        }

        [Fact]
        public void ConstructorSetsSplits()
        {
            string path = "Somepath.someExtension";
            var splits = Substitute.For<ISplits>();
            var file = new SplitsFile(path, splits);

            Assert.Equal(splits, file.Splits);
        }
    }
}
