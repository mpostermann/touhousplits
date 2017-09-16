using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Config;

namespace TouhouSplits.Service.Managers.Config
{
    public class ConfigManager : IConfigManager
    {
        public IList<IGameConfig> AvailableGames { get; private set; }

        public ConfigManager()
        {
            AvailableGames = LoadGamesConfig(); 
        }

        private static IList<IGameConfig> LoadGamesConfig()
        {
            try {
                XDocument gamesXml = XDocument.Load("Games.xml");

                var gamesList = new List<IGameConfig>();
                foreach (XElement gameXml in gamesXml.Root.Element("Games").Elements("Game")) {
                    gamesList.Add(new GameConfig(gameXml));
                }
                return gamesList;
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Could not load Games.xml configuration.", e);
            }
        }
    }
}
