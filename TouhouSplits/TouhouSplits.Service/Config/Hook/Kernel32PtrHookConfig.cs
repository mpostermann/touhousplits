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

            UseThreadStack0 = false;
            if (configElement.Attribute("useThreadStack0") != null) {
                try {
                    UseThreadStack0 = bool.Parse(configElement.Attribute("useThreadStack0").Value);
                }
                catch (Exception e) {
                    throw new ConfigurationErrorsException("Cannot parse \"useThreadStack0\" attribute as a boolean", e);
                }
            }
        }

        private static int[] ParseOffsets(XElement configElement)
        {
            if (configElement.Attribute("offsets") == null ||
                string.IsNullOrEmpty(configElement.Attribute("offsets").Value)) {
                throw new ConfigurationErrorsException("Attribute \"offsets\" is missing");
            }
            try {
                string[] offsetStrings = configElement.Attribute("offsets").Value.Split('|');

                int[] offsets = new int[offsetStrings.Length];
                for (int i = 0; i < offsetStrings.Length; i++) {
                    int offset = int.Parse(offsetStrings[i], System.Globalization.NumberStyles.HexNumber);
                    offsets[i] = offset;
                }
                return offsets;
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Cannot parse \"offsets\" attribute as a series of ints.", e);
            }
        }

        public int[] PointerOffsets { get; }
        public bool UseThreadStack0 { get; }
    }
}
