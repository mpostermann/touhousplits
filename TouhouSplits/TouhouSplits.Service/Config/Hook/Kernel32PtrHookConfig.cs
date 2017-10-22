using System;
using System.Configuration;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config.Hook
{
    public class Kernel32PtrHookConfig : Kernel32HookConfig, IKernel32PtrHookConfig
    {
        public Kernel32PtrHookConfig(XElement configElement)
            : base(configElement)
        {
            PointerOffsets = ParseOffsets(configElement);
        }

        private static int[] ParseOffsets(XElement configElement)
        {
            if (configElement.Attribute("offset") == null ||
                string.IsNullOrEmpty(configElement.Attribute("offset").Value)) {
                throw new ConfigurationErrorsException("Attribute \"offset\" is missing");
            }
            try {
                throw new NotImplementedException();
                //return int.Parse(configElement.Attribute("offset").Value, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Cannot parse \"offset\" attribute as an int.", e);
            }
        }

        public int[] PointerOffsets { get; private set; }
    }
}
