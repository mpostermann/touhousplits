using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using TouhouSplits.Service.Data;

namespace TouhouSplits.Service.Config
{
    public class GameConfig : IGameConfig
    {
        public GameId Id { get; private set; }
        public string GameName { get; private set; }
        public XElement HookConfig { get; private set; }
        public FileInfo FavoriteSplitsList { get; private set; }

        public GameConfig(XElement configElement)
        {
            Id = GetId(configElement);
            GameName = GetGameName(configElement);
            HookConfig = GetHook(configElement);
            FavoriteSplitsList = GetFavoriteSplitsList(configElement);
        }

        private static GameId GetId(XElement configElement)
        {
            if (configElement.Attribute("id") == null ||
                string.IsNullOrEmpty(configElement.Attribute("id").Value)) {
                throw new ConfigurationErrorsException("Attribute \"id\" is missing.");
            }

            string idString = configElement.Attribute("id").Value;
            return new GameId(idString);
        }

        private static string GetGameName(XElement configElement)
        {
            if (configElement.Element("Name") == null ||
                string.IsNullOrEmpty(configElement.Element("Name").Value)) {
                throw new ConfigurationErrorsException("Element \"Name\" is missing.");
            }

            return configElement.Element("Name").Value;
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
