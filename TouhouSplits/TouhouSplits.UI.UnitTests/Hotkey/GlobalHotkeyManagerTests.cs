using NSubstitute;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using TouhouSplits.UI.Hotkey;
using Xunit;

namespace TouhouSplits.UI.UnitTests.Hotkey
{
    public class GlobalHotkeyManagerTests
    {
        private static IGlobalKeyboardHook DefaultKeyboardHook()
        {
            var hook = Substitute.For<IGlobalKeyboardHook>();
            return hook;
        }

        [Fact]
        public void RegisteredHotkeys_List_Is_Empty_By_Default()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            Assert.Equal(0, manager.RegisteredHotkeys.Count);
        }

        [Fact]
        public void RegisterHotkey_Adds_Keys_To_RegisteredHotkeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            Assert.True(manager.RegisteredHotkeys.Contains(Keys.A));
        }

        [Fact]
        public void RegisterHotkey_Throws_Exception_If_Same_Keys_Are_Registered_Multiple_Times()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            Assert.Throws<ArgumentException>(() => manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>()));
        }

        [Fact]
        public void RegisterHotkey_Adds_Keys_If_Key_Was_Previously_Unregistered()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            manager.UnregisterHotkey(Keys.A);
            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            Assert.True(manager.RegisteredHotkeys.Contains(Keys.A));
        }

        [Fact]
        public void UnregisterHotkey_Removes_Keys_From_RegisteredHotkeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            manager.UnregisterHotkey(Keys.A);
            Assert.False(manager.RegisteredHotkeys.Contains(Keys.A));
        }

        [Fact]
        public void UnregisterAllHotkeys_Removes_All_keys_From_RegisteredHotkeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            manager.RegisterHotkey(Keys.B, Substitute.For<ICommand>());
            manager.RegisterHotkey(Keys.C, Substitute.For<ICommand>());

            manager.UnregisterAllHotkeys();
            Assert.Equal(0, manager.RegisteredHotkeys.Count);
        }

        [Fact]
        public void Registered_Command_Is_Executed_When_Hook_Triggers_Event_And_CanExecute_Is_True()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.Received().Execute(null);
        }

        [Fact]
        public void EventArgs_IsHandled_Is_True_If_Command_CanExecute_Is_True()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var command = Substitute.For<ICommand>();
            command.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, command);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            Assert.Equal(eventArgs.Handled, true);
        }

        [Fact]
        public void Registered_Command_Is_Not_Executed_When_Hook_Triggers_Event_And_CanExecute_Is_False()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(false);
            manager.RegisterHotkey(Keys.A, commandMock);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.DidNotReceive().Execute(Arg.Any<object>());
        }

        [Fact]
        public void EventArgs_IsHandled_Is_False_If_Command_CanExecute_Is_False()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var command = Substitute.For<ICommand>();
            command.CanExecute(null).Returns(false);
            manager.RegisterHotkey(Keys.A, command);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            Assert.Equal(eventArgs.Handled, false);
        }

        [Fact]
        public void Unregistered_Command_Is_Not_Executed_After_UnregisterHotkey_Is_Called()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);
            manager.UnregisterHotkey(Keys.A);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.DidNotReceive().Execute(Arg.Any<object>());
        }

        [Fact]
        public void Registered_Command_Is_Not_Executed_When_EventArgs_Is_For_A_Different_Keys()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);
            manager.RegisterHotkey(Keys.B, Substitute.For<ICommand>());

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.B);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.DidNotReceive().Execute(Arg.Any<object>());
        }

        [Fact]
        public void Registered_Commands_Are_Not_Executed_After_Disable_Is_Called()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            manager.Disable();
            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.B);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.DidNotReceive().Execute(Arg.Any<object>());
        }

        [Fact]
        public void Registered_Commands_Are_Executed_After_Enable_Is_Called()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            manager.Disable();
            manager.Enable();
            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.Received().Execute(null);
        }

        [Fact]
        public void RegisterEnableToggleHotkey_Adds_Keys_To_Registered_Hotkeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterEnableToggleHotkey(Keys.P);
            Assert.True(manager.RegisteredHotkeys.Contains(Keys.P));
        }

        [Fact]
        public void UnregisterEnableToggleHotkey_Removes_Keys_From_Registered_Hotkeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterEnableToggleHotkey(Keys.P);
            manager.UnregisterEnableToggleHotkey();
            Assert.False(manager.RegisteredHotkeys.Contains(Keys.P));
        }

        [Fact]
        public void RegisterEnableToggleHotkey_Replaces_Previously_Registered_Hotkey()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterEnableToggleHotkey(Keys.O);
            manager.RegisterEnableToggleHotkey(Keys.P);
            Assert.False(manager.RegisteredHotkeys.Contains(Keys.O));
            Assert.True(manager.RegisteredHotkeys.Contains(Keys.P));
        }

        [Fact]
        public void RegisterEnableToggleHotkey_Throws_Exception_If_Hotkey_Was_Previously_Registered_With_RegisterHotkey()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.P, Substitute.For<ICommand>());
            Assert.Throws<ArgumentException>(() => manager.RegisterEnableToggleHotkey(Keys.P));
        }

        [Fact]
        public void Registered_Commands_Are_Not_Executed_After_EnableToggleHotkey_Is_Pressed_Once()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);
            manager.RegisterEnableToggleHotkey(Keys.P);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            /* Trigger the EnableToggle command */
            var toggleEventArgs = new System.Windows.Forms.KeyEventArgs(Keys.P);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, toggleEventArgs);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.DidNotReceive().Execute(Arg.Any<object>());
        }

        [Fact]
        public void Registered_Commands_Are_Executed_After_EnableToggleHotkey_Is_Pressed_Twice()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);
            manager.RegisterEnableToggleHotkey(Keys.P);

            var commandMock = Substitute.For<ICommand>();
            commandMock.CanExecute(null).Returns(true);
            manager.RegisterHotkey(Keys.A, commandMock);

            /* Trigger then untrigger the EnableToggle command */
            var toggleEventArgs = new System.Windows.Forms.KeyEventArgs(Keys.P);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, toggleEventArgs);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, toggleEventArgs);

            var eventArgs = new System.Windows.Forms.KeyEventArgs(Keys.A);
            hook.KeyDown += Raise.Event<System.Windows.Forms.KeyEventHandler>(this, eventArgs);
            commandMock.Received().Execute(null);
        }
    }
}
