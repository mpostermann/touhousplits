using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public GameId Id { get { throw new NotImplementedException(); } }
        public string GameName { get; private set; }
        public XElement HookConfig { get; private set; }
        public FileInfo FavoriteSplitsList { get; private set; }

        public GameConfig(XElement configElement)
        {
            GameName = GetGameName(configElement);
            HookConfig = GetHook(configElement);
            FavoriteSplitsList = GetFavoriteSplitsList(configElement);
        }

        private static string GetGameName(XElement configElement)
        {
            if (configElement.Attribute("name") == null ||
                string.IsNullOrEmpty(configElement.Attribute("name").Value)) {
                throw new ConfigurationErrorsException("Attribute \"name\" is missing.");
            }

            return configElement.Attribute("name").Value;
        }

        private static XElement GetHook(XElement configElement)
        {
            if (configElement.Element("Hook") == null) {
                throw new ConfigurationErrorsException("Element \"Hook\" is missing.");
            }
            return configElement.Element("Hook");
        }

        private static FileInfo GetFavoriteSplitsList(XElement configElement)
        {
            if (configElement.Attribute("favoriteslist") == null ||
                string.IsNullOrEmpty(configElement.Attribute("favoriteslist").Value)) {
                throw new ConfigurationErrorsException("Element \"favoriteslist\" is missing.");
            }
            return new FileInfo(Path.Combine(
                FilePaths.DIR_FAVORITE_SPLITS_LIST,
                configElement.Attribute("favoriteslist").Value
            ));
        }
    }
}
