using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Reader;

namespace TouhouSplits.Service.Hook.Impl
{
    public class TouhouPtrHookStrategy : Kernel32PtrHookStrategy
    {
        private EncodingEnum _encoding;
        private int _memoryAddress;
        private int[] _pointerOffsets;

        public TouhouPtrHookStrategy(IKernel32PtrHookConfig config, IKernel32MemoryReader memoryReader)
            : base(config, memoryReader)
        {
        }

        /// <summary>
        /// The Touhou games use the last digit of the score to keep track of how many continues the player has
        /// used (for example, a score of 452,981,662 would indicate the player has used 2 continues).
        /// 
        /// Later Touhou games don't keep track of the last digit of the score together with the "main" part of the score.
        /// So using the above example, a score of 452,981,662 would be stored in memory as 45,298,166. To calculate the score
        /// correctly, we need to take the value in memory, multiply it by 10, and then add the number of continues used.
        /// 
        /// Unfortunately I haven't found out how to get the number of coninutes used, so for now all I can do is multiply the
        /// score by 10 lol.
        /// </summary>
        /// <returns></returns>
        public override long GetCurrentScore()
        {
            long score = base.GetCurrentScore();
            return score * 10;
        }
    }
}
