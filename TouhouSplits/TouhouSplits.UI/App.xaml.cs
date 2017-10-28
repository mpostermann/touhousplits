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
            window.Show();
        }
    }
}
