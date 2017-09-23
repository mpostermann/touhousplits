using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config
{
    public class GameConfigTests
    {
        private const string VALID_CONFIG = @"<Game name=""Some game name"">
  <Hook process=""process1|process2|"" address=""0x0069BCA4"" encoding=""int32""/>
</Game>";

        [Fact]
        public void Constructor_Parses_GameName_From_name_Attribute()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Some game name", config.GameName);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_name_Attribute_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("name").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_name_Attribute_Is_Empty()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("name").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Parses_Pipe_Delimited_process_Attribute()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("process1", config.Processes[0]);
            Assert.Equal("process2", config.Processes[1]);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_process_Attribute_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("process").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_process_Attribute_Is_Empty()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("process").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }
        
        [Fact]
        public void Constructor_Loads_Hook_Child_Node()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Hook", config.HookConfig.Name);
            Assert.Equal("0x0069BCA4", config.HookConfig.Attribute("address").Value);
            Assert.Equal("int32", config.HookConfig.Attribute("encoding").Value);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hook_Node_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Element("Hook").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }
    }
}
