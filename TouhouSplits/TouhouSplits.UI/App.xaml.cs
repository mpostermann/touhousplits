﻿using System;
using System.IO;
using System.Windows;
using TouhouSplits.UI.View;
using TouhouSplits.UI.ViewModel;

namespace TouhouSplits.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ApplicationStartup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.DataContext = CreateViewModel(e.Args);
            window.Show();
        }

        private static MainViewModel CreateViewModel(string[] args)
        {
            FileInfo splitsFilePath = TryGetSplitsPath(args);
            if (splitsFilePath != null) {
                return new MainViewModel(splitsFilePath);
            }
            return new MainViewModel();
        }

        private static FileInfo TryGetSplitsPath(string[] args)
        {
            try {
                if (args.Length > 0) {
                    FileInfo path = new FileInfo(args[0]);
                    if (path.Exists) {
                        return path;
                    }
                }
            }
            catch (Exception e) {
                // Do nothing
            }
            return null;
        }
    }
}
