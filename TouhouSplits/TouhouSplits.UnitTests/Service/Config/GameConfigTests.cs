using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using Xunit;

namespace TouhouSplits.UnitTests.Service.Config
{
    public class GameConfigTests
    {
        private const string VALID_CONFIG = @"<Game name=""Some game name"" process=""process1|process2|"">
  <Hook address=""0x0069BCA4"" encoding=""int32""/>
</Game>";

        [Fact]
        public void IsGameNameSet()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Some game name", config.GameName);
        }

        [Fact]
        public void IsExceptionThrownIfGameAttributeIsMissing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("name").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void IsExceptionThrownIfGameNameIsEmptyString()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("name").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void AreProcessesSet()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("process1", config.Processes[0]);
            Assert.Equal("process2", config.Processes[1]);
        }

        [Fact]
        public void IsExceptionThrownIfProcessAttributeIsMissing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("process").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void IsExceptionThrownIfProcessAttributeIsEmptyString()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("process").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }
        
        [Fact]
        public void IsHookConfigSet()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Hook", config.HookConfig.Name);
            Assert.Equal("0x0069BCA4", config.HookConfig.Attribute("address").Value);
            Assert.Equal("int32", config.HookConfig.Attribute("encoding").Value);
        }

        [Fact]
        public void IsExceptionThrownIfHookConfigIsMissing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Element("Hook").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }
    }
}
