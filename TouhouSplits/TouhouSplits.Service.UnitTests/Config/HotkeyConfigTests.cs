using System.Configuration;
using System.Windows.Forms;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config
{
    public class HotkeyConfigTests
    {
        private static XElement DefaultXml()
        {
            string xml = @"<Hotkeys>
    <Hotkey method=""Method 1"" keys=""Add"" />
    <Hotkey method=""Method 2"" keys=""NumPad7"" />
</Hotkeys>";
            return XElement.Parse(xml);
        }

        [Fact]
        public void Constructor_Throws_Exception_If_XElement_Name_Is_Not_Hotkeys()
        {
            var xml = DefaultXml();
            xml.Name = "DifferentRoot";
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hotkey_Node_Is_Missing_method_Attribute()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").Attribute("method").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hotkey_Node_Has_Empty_method_Attribute()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").SetAttributeValue("method", string.Empty);
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Multiple_Hotkey_Nodes_Have_An_Equal_method_Attribute()
        {
            var xml = XElement.Parse(@"<Hotkeys>
    <Hotkey method=""Method 1"" keys=""Add"" />
    <Hotkey method=""Method 1"" keys=""NumPad7"" />
</Hotkeys>");
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hotkey_Node_Is_Missing_keys_Attribute()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").Attribute("keys").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hotkey_Node_Has_keys_Attribute_Which_Cannot_Be_Parsed_As_Keys_Enum()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").SetAttributeValue("keys", "notakey");
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void HasHotkey_Returns_True_For_methods_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.True(config.HasHotkey("Method 1"));
            Assert.True(config.HasHotkey("Method 2"));
        }

        [Fact]
        public void HasHotkey_Returns_False_For_methods_Not_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.False(config.HasHotkey("Nonexistant Method"));
        }

        [Fact]
        public void GetHotkey_Returns_Keys_Enum_For_keys_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.Equal(Keys.Add, config.GetHotkey("Method 1"));
            Assert.Equal(Keys.NumPad7, config.GetHotkey("Method 2"));
        }

        [Fact]
        public void GetHotkey_Returns_Keys_Enum_For_keys_With_Modifiers()
        {
            var xml = XElement.Parse(@"<Hotkeys>
    <Hotkey method=""Method 1"" keys=""Shift+P"" />
</Hotkeys>");
            var config = new HotkeyConfig(xml);
            Assert.Equal(Keys.Shift | Keys.P, config.GetHotkey("Method 1"));
        }
    }
}
