using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.Xml.Linq;
using TouhouSplits.Service.Config;
using TouhouSplits.Service.Data;
using TouhouSplits.Service.Enums;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Config
{
    public class HotkeyConfigTests
    {
        private static XElement DefaultXml()
        {
            string xml = @"<Hotkeys>
    <Hotkey method=""StartOrStopRecordingSplits"" keys=""Add"" />
    <Hotkey method=""SplitToNextSegment"" keys=""NumPad7"" />
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
        public void Constructor_Throws_Exception_If_Hotkey_Node_Is_Missing_keys_Attribute()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").Attribute("keys").Remove();
            Assert.Throws<ConfigurationErrorsException>(() => new HotkeyConfig(xml));
        }

        [Fact]
        public void Constructor_Throws_Exception_If_Hotkey_Node_Has_method_Attribute_Which_Cannot_Be_Parsed_As_HotkeyableMethod_Enum()
        {
            var xml = DefaultXml();
            xml.Element("Hotkey").SetAttributeValue("method", "NonexistantMethod");
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
        public void List_Constructor_Creates_Expected_Configuration()
        {
            var hotkeys = new List<IHotkey>() {
                new HotkeyModel(HotkeyableMethodEnum.SplitToNextSegment, new List<Keys>() { Keys.A }),
                new HotkeyModel(HotkeyableMethodEnum.StartOrStopRecordingSplits, new List<Keys>() { Keys.B, Keys.C })
            };
            var config = new HotkeyConfig(hotkeys);

            Assert.True(config.GetHotkeys(HotkeyableMethodEnum.SplitToNextSegment).Contains(Keys.A));
            Assert.True(config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits).Contains(Keys.B));
            Assert.True(config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits).Contains(Keys.C));
        }

        [Fact]
        public void HasMethod_Returns_True_For_hotkey_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.True(config.HasMethod(Keys.NumPad7));
            Assert.True(config.HasMethod(Keys.Add));
        }

        [Fact]
        public void HasMethod_Returns_False_For_hotkey_Not_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.False(config.HasMethod(Keys.Subtract));
        }

        [Fact]
        public void GetHotkeys_Returns_Keys_Enum_For_keys_In_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.Equal(Keys.Add, config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits)[0]);
            Assert.Equal(Keys.NumPad7, config.GetHotkeys(HotkeyableMethodEnum.SplitToNextSegment)[0]);
        }

        [Fact]
        public void GetHotkeys_Returns_Keys_Enum_For_keys_With_Modifiers()
        {
            var xml = XElement.Parse(@"<Hotkeys>
    <Hotkey method=""StartOrStopRecordingSplits"" keys=""Shift+P"" />
</Hotkeys>");
            var config = new HotkeyConfig(xml);
            Assert.Equal(Keys.Shift | Keys.P, config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits)[0]);
        }

        [Fact]
        public void GetHotkeys_Returns_All_Keys_Enums_For_Methods_With_Multiple_Hotkeys()
        {
            var xml = XElement.Parse(@"<Hotkeys>
    <Hotkey method=""StartOrStopRecordingSplits"" keys=""Add"" />
    <Hotkey method=""StartOrStopRecordingSplits"" keys=""NumPad7"" />
</Hotkeys>");
            var config = new HotkeyConfig(xml);
            Assert.Equal(2, config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits).Count);
            Assert.Contains(Keys.Add, config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits));
            Assert.Contains(Keys.NumPad7, config.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits));
        }

        [Fact]
        public void GetMethod_Returns_Method_For_keys()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.Equal(HotkeyableMethodEnum.StartOrStopRecordingSplits, config.GetMethod(Keys.Add));
            Assert.Equal(HotkeyableMethodEnum.SplitToNextSegment, config.GetMethod(Keys.NumPad7));
        }

        [Fact]
        public void GetMethod_Throws_Exception_For_Nonexistant_keys()
        {
            var config = new HotkeyConfig(DefaultXml());
            Assert.Throws<ConfigurationErrorsException>(() => config.GetMethod(Keys.Subtract));
        }

        [Fact]
        public void ToXml_Generates_Valid_Xml()
        {
            var config = new HotkeyConfig(DefaultXml());

            var xml = config.ToXml();
            var newConfig = new HotkeyConfig(xml);
            Assert.Contains(Keys.Add, newConfig.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits));
            Assert.Contains(Keys.NumPad7, newConfig.GetHotkeys(HotkeyableMethodEnum.SplitToNextSegment));
        }
    }
}
