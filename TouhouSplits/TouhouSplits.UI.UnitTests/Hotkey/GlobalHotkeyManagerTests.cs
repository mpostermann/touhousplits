using NSubstitute;
using System;
using System.Collections.Generic;
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
            hook.HookedKeys.Returns(new List<Keys>());
            return hook;
        }

        [Fact]
        public void RegisterHotkey_Adds_Keys_To_IGlobalKeyboardHook_HookedKeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            Assert.True(hook.HookedKeys.Contains(Keys.A));
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
            Assert.True(hook.HookedKeys.Contains(Keys.A));
        }

        [Fact]
        public void UnregisterHotkey_Removes_Keys_From_IGlobalKeyboardHook_HookedKeys_List()
        {
            var hook = DefaultKeyboardHook();
            var manager = new GlobalHotkeyManager(hook);

            manager.RegisterHotkey(Keys.A, Substitute.For<ICommand>());
            manager.UnregisterHotkey(Keys.A);
            Assert.False(hook.HookedKeys.Contains(Keys.A));
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

    }
}
