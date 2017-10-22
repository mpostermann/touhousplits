using System;
using System.Collections.Generic;
using TouhouSplits.Service.Hook.Impl;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.UnitTests.Hook.Impl
{
    public class FakeKernel32BaseHookStrategy : Kernel32BaseHookStrategy
    {
        public FakeKernel32BaseHookStrategy(ICollection<string> processNames, IKernel32MemoryReader memoryReader)
            : base(processNames, memoryReader)
        { }

        public IGameProcess GetHookedProcess()
        {
            return this.HookedProcess;
        }

        public override long GetCurrentScore()
        {
            throw new NotImplementedException();
        }
    }
}
