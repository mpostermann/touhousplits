using NSubstitute;
using System.IO;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Serialization;
using Xunit;

namespace TouhouSplits.Service.UnitTests
{
    public class SplitsFacadeTests
    {
        [Fact]
        public void SerializeSplits_Invokes_SplitsSerializer()
        {
            var serializerMock = Substitute.For<IFileSerializer<Splits>>();
            var splitsFacade = new SplitsFacade(Substitute.For<IConfigManager>(), serializerMock);

            var splits = Substitute.For<Splits>();
            splitsFacade.SerializeSplits(splits, new FileInfo("new splits path"));
            serializerMock.Received().Serialize(splits, Arg.Is<FileInfo>(n => n.Name == "new splits path"));
        }

        [Fact]
        public void DeserializeSplits_Invokes_SplitsSerializer()
        {
            var serializerMock = Substitute.For<IFileSerializer<Splits>>();
            var splitsFacade = new SplitsFacade(Substitute.For<IConfigManager>(), serializerMock);

            splitsFacade.DeserializeSplits(new FileInfo("new splits path"));
            serializerMock.Received().Deserialize(Arg.Is<FileInfo>(n => n.Name == "new splits path"));
        }

    }
}
