using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NSubstitute;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Enums;
using TouhouSplits.UI.Model;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Model
{
    public class HotkeyConfigModelTests
    {
        private IConfigManager DefaultConfig()
        {
            var config = Substitute.For<IConfigManager>();
            config.Hotkeys.GetHotkeys(HotkeyableMethodEnum.ToggleHotkeys).Returns(new List<Keys>() {Keys.A});
            config.Hotkeys.GetHotkeys(HotkeyableMethodEnum.StartOrStopRecordingSplits).Returns(new List<Keys>() {Keys.B});
            config.Hotkeys.GetHotkeys(HotkeyableMethodEnum.SplitToNextSegment).Returns(new List<Keys>() {Keys.C, Keys.D});

            return config;
        }

        [Theory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.B, true)]
        [InlineData(Keys.C, true)]
        [InlineData(Keys.D, true)]
        [InlineData(Keys.E, false)]
        public void HasHotkey_Returns_True_For_Keys_Mapped_To_Method(Keys keys, bool expectedValue)
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            Assert.Equal(expectedValue, model.HasHotkey(keys));
        }

        [Theory]
        [InlineData(Keys.A, HotkeyableMethodEnum.ToggleHotkeys)]
        [InlineData(Keys.B, HotkeyableMethodEnum.StartOrStopRecordingSplits)]
        [InlineData(Keys.C, HotkeyableMethodEnum.SplitToNextSegment)]
        [InlineData(Keys.D, HotkeyableMethodEnum.SplitToNextSegment)]
        [InlineData(Keys.E, null)]
        public void GetHotkeyOrNull_Returns_Method_Mapped_To_Key(Keys keys, HotkeyableMethodEnum? expectedMethod)
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            Assert.Equal(expectedMethod, model.GetHotkeyOrNull(keys)?.Method);
        }

        [Fact]
        public void RemoveHotkey_Removes_Keys_From_Method()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            model.RemoveHotkey(Keys.C);
            Assert.False(model.HasHotkey(Keys.C));
            Assert.DoesNotContain(Keys.C, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.SplitToNextSegment).Keys);
        }

        [Fact]
        public void RemoveHotkey_Sets_KeysNone_If_Removing_Last_Keys_From_Method()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            model.RemoveHotkey(Keys.A);
            Assert.DoesNotContain(Keys.A, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.ToggleHotkeys).Keys);
            Assert.Contains(Keys.None, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.ToggleHotkeys).Keys);
        }

        [Fact]
        public void AddEmptyHotkey_Adds_KeysNone_To_Method()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            model.AddEmptyHotkey(Keys.A);
            Assert.Contains(Keys.None, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.ToggleHotkeys).Keys);
        }

        [Fact]
        public void AddEmptyHotkey_Doesnt_Add_If_Method_Already_Contains_KeysNone()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            // Add an empty hotkey twice, but expect it to only show up once
            model.AddEmptyHotkey(Keys.A);
            model.AddEmptyHotkey(Keys.A);
            Assert.Contains(Keys.None, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.ToggleHotkeys).Keys);
            Assert.Equal(2, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.ToggleHotkeys).Keys.Count);
        }

        [Fact]
        public void EditHotkey_Edits_Keys_In_Place()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            model.EditHotkey(Keys.C, Keys.Add);

            // Expect the new hotkey to show up in the same order that C was originally in
            Assert.Equal(0, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.SplitToNextSegment).Keys.IndexOf(Keys.Add));
            Assert.Equal(2, model.Hotkeys.First(n => n.Method == HotkeyableMethodEnum.SplitToNextSegment).Keys.Count);
        }

        [Fact]
        public void EditHotkey_Throws_Exception_If_newKeys_Is_Already_Mapped()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            Assert.Throws<ArgumentException>(() => model.EditHotkey(Keys.C, Keys.A));
        }

        [Fact]
        public void PersistChanges_Persists_Hotkeys()
        {
            var config = DefaultConfig();
            var model = new HotkeysConfigModel(config);

            model.PersistChanges();
            config.Received().UpdateAndPersistHotkeys(model.Hotkeys);
        }
    }
}
