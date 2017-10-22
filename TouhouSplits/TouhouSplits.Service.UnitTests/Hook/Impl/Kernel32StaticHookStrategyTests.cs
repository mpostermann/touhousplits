using NSubstitute;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Hook.Impl;
using TouhouSplits.Service.Hook.Reader;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Hook.Impl
{
    public class Kernel32StaticHookStrategyTests
    {
        private static IKernel32HookConfig DefaultConfig(string processName, int address, EncodingEnum encoding) {
            var config = Substitute.For<IKernel32HookConfig>();
            config.ProcessNames.Returns(new[] { processName });
            config.Address.Returns(address);
            config.Encoding.Returns(encoding);
            return config;
        }

        private static IKernel32MemoryReader DefaultMemoryReader(string processName) {
            IGameProcess[] processes = new IGameProcess[1];
            processes[0] = Substitute.For<IGameProcess>();
            processes[0].HasExited.Returns(false);

            var reader = Substitute.For<IKernel32MemoryReader>();
            reader.GetProcessesByName(processName).Returns(processes);
            return reader;
        }

        [Fact]
        public void GetCurrentScore_Hooks_Game_If_Process_Is_Not_Hooked()
        {
            var strategy = new Kernel32StaticHookStrategy(
                DefaultConfig("process1", 12345, EncodingEnum.int32),
                DefaultMemoryReader("process1"));

            strategy.GetCurrentScore();
            Assert.True(strategy.IsHooked);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(123456)]
        public void GetCurrentScore_Returns_MemoryReader_ReadInt_Value_If_Encoding_Enum_Is_Int32(int expectedScore)
        {
            var memoryReader = DefaultMemoryReader("process1");
            var strategy = new Kernel32StaticHookStrategy(
                DefaultConfig("process1", 12345, EncodingEnum.int32),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 12345)
                .Returns(expectedScore);
            Assert.Equal(expectedScore, strategy.GetCurrentScore());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(987654321)]
        public void GetCurrentScore_Returns_MemoryReader_ReadLong_Value_If_Encoding_Enum_Is_Int64(int expectedScore)
        {
            var memoryReader = DefaultMemoryReader("process1");
            var strategy = new Kernel32StaticHookStrategy(
                DefaultConfig("process1", 12345, EncodingEnum.int64),
                memoryReader);

            memoryReader
                .ReadLong(memoryReader.GetProcessesByName("process1")[0], 12345)
                .Returns(expectedScore);
            Assert.Equal(expectedScore, strategy.GetCurrentScore());
        }
    }
}
