using NSubstitute;
using System;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook;
using TouhouSplits.Service.Hook.Impl;
using TouhouSplits.Service.Hook.Reader;
using Xunit;

namespace TouhouSplits.Service.UnitTests.Hook.Impl
{
    public class Kernel32PtrHookStrategyTests
    {
        private static IKernel32PtrHookConfig DefaultConfig(string processName, int address, int[] offsets, EncodingEnum encoding, bool useThreadStack0 = false)
        {
            var config = Substitute.For<IKernel32PtrHookConfig>();
            config.ProcessNames.Returns(new[] { processName });
            config.Address.Returns(address);
            config.Encoding.Returns(encoding);
            config.PointerOffsets.Returns(offsets);
            config.UseThreadStack0.Returns(useThreadStack0);
            return config;
        }

        private static IKernel32MemoryReader DefaultMemoryReader(string processName, int baseAddress, int threadStack0Address)
        {
            IGameProcess[] processes = new IGameProcess[1];
            processes[0] = Substitute.For<IGameProcess>();
            processes[0].HasExited.Returns(false);
            processes[0].BaseAddress.Returns(new IntPtr(baseAddress));
            processes[0].ThreadStack0Address.Returns(new IntPtr(threadStack0Address));

            var reader = Substitute.For<IKernel32MemoryReader>();
            reader.GetProcessesByName(processName).Returns(processes);
            return reader;
        }

        [Fact]
        public void GetCurrentScore_Hooks_Game_If_Process_Is_Not_Hooked()
        {
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new [] { 8 }, EncodingEnum.int32),
                DefaultMemoryReader("process1", 98765, 54321));

            strategy.GetCurrentScore();
            Assert.True(strategy.IsHooked);
        }

        [Theory]
        [InlineData(12345, 98765)]
        [InlineData(468713, 1621)]
        public void GetCurrentScore_Reads_Int_From_Pointed_Address_If_PointerOffsets_Is_Empty_And_EncodingEnum_Is_Int32(int pointerAddress, int baseAddress)
        {
            var memoryReader = DefaultMemoryReader("process1", baseAddress, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", pointerAddress, new int[0], EncodingEnum.int32),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], pointerAddress + baseAddress)
                .Returns(67890);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 67890)
                .Returns(589918964);

            Assert.Equal(589918964, strategy.GetCurrentScore());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(16)]
        public void GetCurrentScore_Reads_Int_From_Pointed_Address_With_One_PointerOffset_And_EncodingEnum_Is_Int32(int offset)
        {
            var memoryReader = DefaultMemoryReader("process1", 98765, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new[] { offset }, EncodingEnum.int32),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 12345 + 98765)
                .Returns(67890);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 67890 + offset)
                .Returns(589918964);

            Assert.Equal(589918964, strategy.GetCurrentScore());
        }

        [Fact]
        public void GetCurrentScore_Reads_Int_From_Pointed_Address_With_Many_PointerOffsets_And_EncodingEnum_Is_Int32()
        {
            var memoryReader = DefaultMemoryReader("process1", 98765, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new[] { 16, 32, 0, 20 }, EncodingEnum.int32),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 12345 + 98765)
                .Returns(23456);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 23456 + 16)
                .Returns(34567);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 34567 + 32)
                .Returns(45678);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 45678 + 0)
                .Returns(56789);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 56789 + 20)
                .Returns(89498198);

            Assert.Equal(89498198, strategy.GetCurrentScore());
        }

        [Theory]
        [InlineData(12345, 98765)]
        [InlineData(468713, 1621)]
        public void GetCurrentScore_Reads_Long_From_Pointed_Address_If_PointerOffsets_Is_Empty_And_EncodingEnum_Is_Int64(int pointerAddress, int baseAddress)
        {
            var memoryReader = DefaultMemoryReader("process1", baseAddress, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", pointerAddress, new int[0], EncodingEnum.int64),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], pointerAddress + baseAddress)
                .Returns(67890);
            memoryReader
                .ReadLong(memoryReader.GetProcessesByName("process1")[0], 67890)
                .Returns(589918964);

            Assert.Equal(589918964, strategy.GetCurrentScore());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(16)]
        public void GetCurrentScore_Reads_Int_From_Pointed_Address_With_One_PointerOffset_And_EncodingEnum_Is_Int64(int offset)
        {
            var memoryReader = DefaultMemoryReader("process1", 98765, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new[] { offset }, EncodingEnum.int64),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 12345 + 98765)
                .Returns(67890);
            memoryReader
                .ReadLong(memoryReader.GetProcessesByName("process1")[0], 67890 + offset)
                .Returns(589918964);

            Assert.Equal(589918964, strategy.GetCurrentScore());
        }

        [Fact]
        public void GetCurrentScore_Reads_Int_From_Pointed_Address_With_Many_PointerOffsets_And_EncodingEnum_Is_Int64()
        {
            var memoryReader = DefaultMemoryReader("process1", 98765, 54321);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new[] { 16, 32, 0, 20 }, EncodingEnum.int64),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 12345 + 98765)
                .Returns(23456);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 23456 + 16)
                .Returns(34567);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 34567 + 32)
                .Returns(45678);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 45678 + 0)
                .Returns(56789);
            memoryReader
                .ReadLong(memoryReader.GetProcessesByName("process1")[0], 56789 + 20)
                .Returns(89498198);

            Assert.Equal(89498198, strategy.GetCurrentScore());
        }

        [Theory]
        [InlineData(0, 54321)]
        [InlineData(8, 54321)]
        [InlineData(16, 91919)]
        public void GetCurrentScore_Reads_Int_From_ThreadStack_Address(int offset, int threadStack0Address)
        {
            var memoryReader = DefaultMemoryReader("process1", 98765, threadStack0Address);
            var strategy = new Kernel32PtrHookStrategy(
                DefaultConfig("process1", 12345, new[] { offset }, EncodingEnum.int32, true),
                memoryReader);

            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], -12345 + threadStack0Address)
                .Returns(67890);
            memoryReader
                .ReadInt(memoryReader.GetProcessesByName("process1")[0], 67890 + offset)
                .Returns(589918964);

            Assert.Equal(589918964, strategy.GetCurrentScore());
        }
    }
}
