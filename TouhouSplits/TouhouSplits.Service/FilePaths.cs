using System;
using System.IO;

namespace TouhouSplits.Service
{
    public static class FilePaths
    {
        public const string EXT_SPLITS_FILE = ".tsf";
        public const string EXT_FAVORITE_SPLITS_LIST_FILE = ".trs";

        private static readonly string _dir_app_config_default = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TouhouSplits");
        private static string _dir_app_config = _dir_app_config_default;
        public static string DIR_APP_CONFIG { get { return _dir_app_config; } }

        public static string DIR_FAVORITE_SPLITS_LIST { get { return Path.Combine(DIR_APP_CONFIG, "Favorites"); } }

        public static void SetAppConfigDirectory(string path)
        {
            _dir_app_config = path;
        }

        public static void ResetAppConfigDirectoryToDefault()
        {
            _dir_app_config = _dir_app_config_default;
        }
    }
}
