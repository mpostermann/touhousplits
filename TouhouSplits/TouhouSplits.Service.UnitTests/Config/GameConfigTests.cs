using System.Configuration;
using System.IO;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config
{
    public class GameConfigTests
    {
        private const string VALID_CONFIG = @"<Game id=""Some game id"" favoriteslist=""gamename.trs"">
  <Name>Some game name</Name>
  <Hook strategy=""Kernel32HookStrategy"" process=""process1|process2"" address=""0x0069BCA4"" encoding=""int32""/>
</Game>";

        [Fact]
        public void Constructor_Parses_Id_From_id_Attribute()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal(new GameId("Some game id"), config.Id);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_id_Attribute_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("id").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_id_Attribute_Is_Empty()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("id").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Parses_GameName_From_name_Element()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Some game name", config.GameName);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_name_Element_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Element("Name").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_name_Element_Is_Empty()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Element("Name").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Parse_Favorites_Filepath_from_favoriteslist_Attribute()
        {
            FilePaths.SetAppConfigDirectory("root");
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            FilePaths.ResetAppConfigDirectoryToDefault();

            var expectedFile = new FileInfo(@"root\Favorites\gamename.trs");
            Assert.Equal(expectedFile.FullName, config.FavoriteSplitsList.FullName);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_favoriteslist_Attribute_Is_Missing()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("favoriteslist").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_favoriteslist_Attribute_Is_Empty()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            xml.Attribute("favoriteslist").Value = string.Empty;
            Assert.Throws<ConfigurationErrorsException>(() => new GameConfig(xml));
        }

        [Fact]
        public void Constructor_Loads_Hook_Child_Node()
        {
            XElement xml = XElement.Parse(VALID_CONFIG);
            var config = new GameConfig(xml);
            Assert.Equal("Hook", config.HookConfig.Name);
            Assert.Equal("Kernel32HookStrategy", config.HookConfig.Attribute("strategy").Value);
            Assert.Equal("process1|process2", config.HookConfig.Attribute("process").Value);
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
