using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Service.Config.Hook;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config.Hook
{
    public class Kernel32PtrHookConfigTests
    {
        private static XElement DefaultValidXml()
        {
            return XElement.Parse(@"<Hook strategy=""Kernel32PtrHookStrategy"" process=""process1|process2"" address=""0069BCA4"" offsets=""8|20"" encoding=""int32""/>");
        }

        [Fact]
        public void Constructor_Throws_Exception_If_offsets_Attribute_Is_Missing()
        {
            XElement xml = DefaultValidXml();
            xml.Attribute("offsets").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32PtrHookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_offsets_Attribute_Is_Empty()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("offsets", string.Empty);
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32PtrHookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_offsets_Attribute_Cannot_Be_Parsed_As_Hex_Int()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("offsets", "GA");
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32PtrHookConfig(xml));
        }

        [Fact]
        public void PointerOffsets_Returns_Pipe_Delimited_Ints_From_offsets_Attribute()
        {
            var config = new Kernel32PtrHookConfig(DefaultValidXml());
            Assert.Equal(2, config.PointerOffsets.Length);
            Assert.Equal(8, config.PointerOffsets[0]);
            Assert.Equal(32, config.PointerOffsets[1]);
        }
    }
}
