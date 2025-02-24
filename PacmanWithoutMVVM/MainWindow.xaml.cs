using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            MainFrame.Navigate(new MainMenuePage(this)); // Startet mit dem Menü
        }
        public void ShowPage(Page page)
        {
            MainFrame.Navigate(page);
        }

        public int ChosenSkin { get; set; } = 1;  // Default skin 1
        public string PlayerName { get; set; } = "Player"; //falls leer oder leerzeichen
    }
}