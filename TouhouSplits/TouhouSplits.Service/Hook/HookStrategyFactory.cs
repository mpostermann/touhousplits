using System;
using System.Xml.Linq;
using TouhouSplits.Service.Config.Hook;
using TouhouSplits.Service.Hook.Impl;
using TouhouSplits.Service.Hook.Reader;

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
                case "kernel32statichookstrategy":
                    var config = new Kernel32HookConfig(configElement);
                    return new Kernel32StaticHookStrategy(config, new Kernel32MemoryReader());
                case "touhoustatichookstrategy":
                    var thStaticConfig = new Kernel32HookConfig(configElement);
                    return new TouhouStaticHookStrategy(thStaticConfig, new Kernel32MemoryReader());
                case "kernel32ptrhookstrategy":
                    var ptrconfig = new Kernel32PtrHookConfig(configElement);
                    return new Kernel32PtrHookStrategy(ptrconfig, new Kernel32MemoryReader());
                case "touhouptrhookstrategy":
                    var thPtrConfig = new Kernel32PtrHookConfig(configElement);
                    return new TouhouPtrHookStrategy(thPtrConfig, new Kernel32MemoryReader());
                default:
                    throw new NotSupportedException(string.Format("Unknown hook strategy {0}", strategyType));
            }
        }
    }
}
