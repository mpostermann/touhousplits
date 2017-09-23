using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public string GameName { get; private set; }
        public XElement HookConfig { get; private set; }
        public IList<FileInfo> RecentSplitsPaths { get; private set; }

        public GameConfig(XElement configElement)
        {
            GameName = GetGameName(configElement);
            HookConfig = GetHook(configElement);
            RecentSplitsPaths = GetRecentFiles();
        }

        private static string GetGameName(XElement configElement)
        {
            if (configElement.Attribute("name") == null ||
                string.IsNullOrEmpty(configElement.Attribute("name").Value)) {
                throw new ConfigurationErrorsException("Attribute \"name\" is missing.");
            }

            return configElement.Attribute("name").Value;
        }

        private XElement GetHook(XElement configElement)
        {
            if (configElement.Element("Hook") == null) {
                throw new ConfigurationErrorsException("Element \"Hook\" is missing.");
            }
            return configElement.Element("Hook");
        }

        private IList<FileInfo> GetRecentFiles()
        {
            throw new NotImplementedException();
        }
    }
}
