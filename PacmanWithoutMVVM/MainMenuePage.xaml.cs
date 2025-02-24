using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacmanWithoutMVVM
{
    /// <summary>
    /// Interaktionslogik für MainMenuePage.xaml
    /// </summary>
    public partial class MainMenuePage : Page
    {
        public MainMenuePage(MainWindow mainWindow)
        {
            InitializeComponent();
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new PopUpFensterMitNamen());
        }

        private void Ranking_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new RankingPage());
        }

        private void btn_skins_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new SkinPage());
        }

        private void btn_leave_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
