using NSubstitute;
using System;
using TouhouSplits.Service.Hook.Reader;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Hook.Impl
{
    public class Kernel32BaseHookStrategyTests
    {
        private static string[] DefaultProcessNames()
        {
            return new[] { "process1", "process2", "process3" };
        }

        private static void SetProcessAsRunning(IKernel32MemoryReader memReader, string processId)
        {
            IGameProcess[] processes = new IGameProcess[1];
            processes[0] = Substitute.For<IGameProcess>();
            processes[0].HasExited.Returns(false);

            memReader.GetProcessesByName(processId).Returns(processes);
        }

        [Fact]
        public void GameIsRunning_Returns_False_If_Process_Is_Not_Running()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            Assert.Equal(false, strategy.GameIsRunning());
        }

        [Theory]
        [InlineData("process1")]
        [InlineData("process2")]
        [InlineData("process3")]
        public void GameIsRunning_Returns_True_If_Process_Is_Running(string processName)
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            SetProcessAsRunning(memReader, processName);
            Assert.Equal(true, strategy.GameIsRunning());
        }

        [Fact]
        public void IsHooked_Returns_True_After_Hook_Is_Executed()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            SetProcessAsRunning(memReader, "process1");
            strategy.Hook();
            Assert.True(strategy.IsHooked);
        }

        [Fact]
        public void IsHooked_Returns_False_After_Unhook_Is_Executed()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            SetProcessAsRunning(memReader, "process1");
            strategy.Hook();
            strategy.Unhook();
            Assert.False(strategy.IsHooked);
        }

        [Fact]
        public void Hook_Throws_Exception_If_Process_Is_Not_Running()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            memReader.GetProcessesByName(Arg.Any<string>()).Returns(new IGameProcess[0]);
            Assert.Throws<InvalidOperationException>(() => strategy.Hook());
        }

        [Fact]
        public void Hook_Sets_HookedProcess_To_Currently_Running_Process()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            SetProcessAsRunning(memReader, "process1");
            var expectedProcess = memReader.GetProcessesByName("process1")[0];
            strategy.Hook();
            Assert.Equal(expectedProcess, strategy.GetHookedProcess());
        }

        [Fact]
        public void Unook_Sets_HookedProcess_To_Null()
        {
            var memReader = Substitute.For<IKernel32MemoryReader>();
            var strategy = new FakeKernel32BaseHookStrategy(DefaultProcessNames(), memReader);

            SetProcessAsRunning(memReader, "process1");
            strategy.Hook();
            strategy.Unhook();
            Assert.Null(strategy.GetHookedProcess());
        }
    }
}
