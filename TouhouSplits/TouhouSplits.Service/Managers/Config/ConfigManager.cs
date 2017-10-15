using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            try {
                FileInfo filepath = new FileInfo(Path.Combine(FilePaths.DIR_APP_CONFIG, "Hotkeys.xml"));
                XDocument configDoc;
                if (!filepath.Exists) {
                    configDoc = CreateDefaultHotkeyXml(filepath);
                }
                else {
                    configDoc = XDocument.Load(filepath.FullName);
                }

                return new HotkeyConfig(configDoc.Root);
            }
            catch (Exception e) {
                throw new ConfigurationErrorsException("Could not load Hotkeys.xml configuration.", e);
            }
        }

        private static XDocument CreateDefaultHotkeyXml(FileInfo filepath)
        {
            var configDoc = XDocument.Parse(Properties.Resources.Hotkeys);
            try {
                configDoc.Save(filepath.FullName);
            }
            catch {
                //Do nothing
            }
            return configDoc;
        }
    }
}
