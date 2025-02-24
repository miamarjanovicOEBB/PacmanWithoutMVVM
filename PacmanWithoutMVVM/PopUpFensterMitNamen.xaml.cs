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
    /// Interaktionslogik für PopUpFensterMitNamen.xaml
    /// </summary>
    public partial class PopUpFensterMitNamen : Page
    {
        private MainWindow mainWindow;
        public PopUpFensterMitNamen()
        {
            InitializeComponent(); //UI
        }

        private void gobackbtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow));
        }

        private void startGamebtn_Click(object sender, RoutedEventArgs e) //kurze if-else
        {
            MainWindow.Instance.PlayerName = string.IsNullOrWhiteSpace(txtPlayerName.Text)
                                      ? "Player" // if: Falls das Feld leer ist oder nur Leerzeichen enthält
                                      : txtPlayerName.Text; //else
            MainWindow.Instance.ShowPage(new PlayPage());
        }
    }
}
