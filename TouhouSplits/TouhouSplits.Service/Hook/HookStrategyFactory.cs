using System;
using System.Xml.Linq;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Impl;

namespace TouhouSplits.Service.Hook
{
    public class HookStrategyFactory : IHookStrategyFactory
    {
        private HookStrategyFactory()
        {
            // Empty private constructor to hide the default public constructor
        }

        private static IHookStrategyFactory _instance;
        public static IHookStrategyFactory GetInstance()
        {
            if (_instance == null) {
                _instance = new HookStrategyFactory();
            }
            return _instance;
        }

        public IHookStrategy Create(XElement configElement)
        {
            string strategyType = configElement.Attribute("strategy").Value.Trim().ToLower();
            switch (strategyType) {
                case "kernel32hookstrategy":
                    var config = new Kernel32HookConfig(configElement);
                    return new Kernel32HookStrategy(config);
                default:
                    throw new NotSupportedException(string.Format("Unknown hook strategy {0}", strategyType));
            }
        }
    }
}
