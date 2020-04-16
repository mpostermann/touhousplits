using System;

namespace TouhouSplits.Service.Hook.Impl
{
    public class NonHookingStrategy : IHookStrategy
    {
        public bool IsHooked => throw new NotImplementedException();

        public bool GameIsRunning()
        {
            return false;
        }

        public long GetCurrentScore()
        {
            return 0;
        }

        public void Hook()
        {
            return;
        }

        public void Unhook()
        {
            return;
        }
    }
}
