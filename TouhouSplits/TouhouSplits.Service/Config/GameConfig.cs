using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public string GameName { get; private set; }
        public IList<string> Processes { get; private set; }
        public XElement HookConfig { get; private set; }

        public GameConfig(XElement configElement)
        {
            GameName = GetGameName(configElement);
            Processes = GetProcesses(configElement);
            HookConfig = GetHook(configElement);
        }

        private static string GetGameName(XElement configElement)
        {
            if (configElement.Attribute("name") == null ||
                string.IsNullOrEmpty(configElement.Attribute("name").Value)) {
                throw new ConfigurationErrorsException("Attribute \"name\" is missing.");
            }

            return configElement.Attribute("name").Value;
        }

        private static ReadOnlyCollection<string> GetProcesses(XElement configElement)
        {
            if (configElement.Attribute("process") == null ||
                string.IsNullOrEmpty(configElement.Attribute("process").Value)) {
                throw new ConfigurationErrorsException("Attribute \"process\" is missing.");
            }

            var processes = configElement.Attribute("process").Value.Split('|');
            return new ReadOnlyCollection<string>(processes);
        }

        private XElement GetHook(XElement configElement)
        {
            if (configElement.Element("Hook") == null) {
                throw new ConfigurationErrorsException("Element \"Hook\" is missing.");
            }
            return configElement.Element("Hook");
        }
    }
}
