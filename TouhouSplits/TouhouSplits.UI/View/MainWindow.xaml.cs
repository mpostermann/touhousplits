using System.Windows;
using System.Windows.Controls;
using TouhouSplits.UI.Model;
using TouhouSplits.UI.ViewModel;

namespace TouhouSplits.UI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel dataContext)
        {
            this.InitializeComponent();
            this.DataContext = dataContext;

            dataContext.MainModel.PropertyChanged += MainModel_PropertyChanged;
        }

        private void MainModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var model = (PersonalBestTracker)sender;
            if (e.PropertyName == "IsHookable")
            {
                var grid = (DataGrid) this.FindName("ScoreDataGrid");
                if (model.IsHookable)
                    grid.Columns[1].Visibility = Visibility.Visible;
                else
                    grid.Columns[1].Visibility = Visibility.Hidden;
            }
        }
    }
}
