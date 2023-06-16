using System;
using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Service.Hook;

namespace TouhouSplits.Service.Config.Hook
{
    public class Kernel32HookConfig : IKernel32HookConfig
    {
        public int Address { get; private set; }
        public EncodingEnum Encoding { get; private set; }
        public int Length { get; private set; }
        public string[] ProcessNames { get; private set; }

        public Kernel32HookConfig(XElement configElement)
        {
            Address = ParseAddress(configElement);
            Encoding = ParseEncoding(configElement);
            Length = ParseLength(configElement, Encoding);
            ProcessNames = ParseProcessNames(configElement);
        }

        private static int ParseAddress(XElement configElement)
        {
            if (configElement.Attribute("address") == null ||
                string.IsNullOrEmpty(configElement.Attribute("address").Value)) {
                throw new ConfigurationErrorsException("Attribute \"address\" is missing");
            }
            try {
                return int.Parse(configElement.Attribute("address").Value, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Cannot parse \"address\" attribute as an int.", e);
            }
        }

        private static EncodingEnum ParseEncoding(XElement configElement)
        {
            if (configElement.Attribute("encoding") == null ||
                string.IsNullOrEmpty(configElement.Attribute("encoding").Value)) {
                throw new ConfigurationErrorsException("Attribute \"encoding\" is missing");
            }
            string parsedEncoding = configElement.Attribute("encoding").Value.Trim().ToLower();
            switch (parsedEncoding) {
                case "int32":
                    return EncodingEnum.int32;
                case "int64":
                    return EncodingEnum.int64;
                case "arrayofnumbers":
                    return EncodingEnum.arrayOfNumbers;
                default:
                    throw new ConfigurationErrorsException(string.Format("Encoding type \"{0}\" is not supported", parsedEncoding));
            }
        }

        private static int ParseLength(XElement configElement, EncodingEnum encoding)
        {
            if (encoding != EncodingEnum.arrayOfNumbers) {
                return 0;
            }

            if (configElement.Attribute("length") == null ||
                string.IsNullOrEmpty(configElement.Attribute("length").Value)) {
                throw new ConfigurationErrorsException("Attribute \"length\" is required when encoding is arrayOfNumbers");
            }

            int length = 0;
            try {
                length = int.Parse(configElement.Attribute("length").Value);
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Cannot parse \"length\" attribute as an int.", e);
            }

            if (length < 1) {
                throw new ConfigurationErrorsException("Attribute \"length\" must be 1 or greater.");
            }

            return length;
        }

        private static string[] ParseProcessNames(XElement configElement)
        {
            if (configElement.Attribute("process") == null ||
                string.IsNullOrEmpty(configElement.Attribute("process").Value)) {
                throw new ConfigurationErrorsException("Attribute \"process\" is missing");
            }
            return configElement.Attribute("process").Value.Split('|');
        }
    }
}
