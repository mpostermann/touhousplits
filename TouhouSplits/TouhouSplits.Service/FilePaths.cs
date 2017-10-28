using System;
using System.IO;
using System.Reflection;

namespace TouhouSplits.Service
{
    public static class FilePaths
    {
        public const string EXT_SPLITS_FILE = ".tsf";
        public const string EXT_FAVORITE_SPLITS_LIST_FILE = ".trs";
        public const string EXT_CRASH_FILE = ".crash";

        private static string _dir_exection_path_default = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _dir_execution_path = _dir_exection_path_default;
        public static string DIR_EXECUTION_PATH { get { return _dir_execution_path; } }

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

        public static void SetExecutionDirectory(string path)
        {
            _dir_execution_path = path;
        }

        public static void ResetExecutionDirectoryToDefault()
        {
            _dir_execution_path = _dir_exection_path_default;
        }
    }
}
