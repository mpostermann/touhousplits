using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Service.Config.Hook;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config.Hook
{
    public class Kernel32HookConfigTests
    {
        private static XElement DefaultValidXml() {
            return XElement.Parse(@"<Hook strategy=""Kernel32StaticHookStrategy"" process=""process1|process2"" address=""0069BCA4"" encoding=""int32""/>");
        }

        [Fact]
        public void Address_Returns_Int_From_address_Attribute()
        {
            XElement xml = DefaultValidXml();
            var config = new Kernel32HookConfig(xml);
            Assert.Equal(0x0069BCA4, config.Address);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_address_Attribute_Is_Missing()
        {
            XElement xml = DefaultValidXml();
            xml.Attribute("address").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_address_Attribute_Is_Empty()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("address", string.Empty);
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_address_Attribute_Is_Not_An_Int()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("address", "One");
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Theory]
        [InlineData("int32")]
        [InlineData("int64")]
        public void Encoding_Returns_Encoding_From_encoding_Attribute(string encoding)
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("encoding", encoding);
            var config = new Kernel32HookConfig(xml);
            Assert.Equal(encoding, config.Encoding.ToString().Trim().ToLower());
        }

        [Fact]
        public void Constructor_Throws_Exception_If_encoding_Attribute_Is_Missing()
        {
            XElement xml = DefaultValidXml();
            xml.Attribute("encoding").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_encoding_Attribute_Is_Empty()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("encoding", string.Empty);
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_encoding_Attribute_Is_Not_Parseable()
        {
            XElement xml = DefaultValidXml();
            xml.SetAttributeValue("encoding", "int33");
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Parses_Pipe_Delimited_process_Attribute()
        {
            XElement xml = DefaultValidXml();
            var config = new Kernel32HookConfig(xml);
            Assert.Equal("process1", config.ProcessNames[0]);
            Assert.Equal("process2", config.ProcessNames[1]);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_process_Attribute_Is_Missing()
        {
            XElement xml = DefaultValidXml();
            xml.Attribute("process").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_process_Attribute_Is_Empty()
        {
            XElement xml = DefaultValidXml();
            xml.Attribute("process").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new Kernel32HookConfig(xml));
        }
    }
}
