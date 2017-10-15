using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using TouhouSplits.Manager.Config;
using TouhouSplits.Service.Config;

namespace TouhouSplits.Service.Managers.Config
{
    public class ConfigManager : IConfigManager
    {
        public IList<IGameConfig> AvailableGames { get; private set; }
        public IHotkeyConfig Hotkeys { get; private set; }

        public ConfigManager()
        {
            AvailableGames = LoadGamesConfig();
            Hotkeys = LoadHotkeyConfig();
        }

        private static IList<IGameConfig> LoadGamesConfig()
        {
            try {
                XDocument gamesXml = XDocument.Load("Games.xml");

                var gamesList = new List<IGameConfig>();
                foreach (XElement gameXml in gamesXml.Root.Element("Games").Elements("Game")) {
                    var config = new GameConfig(gameXml);
                    if (gamesList.Any(n => n.Id == config.Id)) {
                        throw new ConfigurationErrorsException(string.Format("Game with duplicate Id \"{0}\" found", config.Id));
                    }
                    gamesList.Add(new GameConfig(gameXml));
                }
                return gamesList;
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Could not load Games.xml configuration.", e);
            }
        }

        private static IHotkeyConfig LoadHotkeyConfig()
        {
            throw new NotImplementedException();
        }
    }
}
