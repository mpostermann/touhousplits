using System;
using System.Xml.Linq;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Config.Hook
{
    public class Kernel32HookConfig : IKernel32HookConfig
    {
        public int Address { get; private set; }
        public EncodingEnum Encoding { get; private set; }
        public string[] ProcessNames { get; private set; }

        public Kernel32HookConfig(XElement configElement)
        {
            throw new NotImplementedException();
        }
    }
}
