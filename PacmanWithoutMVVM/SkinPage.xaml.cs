using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PacmanWithoutMVVM
{
    /// <summary>
    /// Interaktionslogik für SkinPage.xaml
    /// </summary>
    public partial class SkinPage : Page
    {
        private MainWindow mainWindow;
        private int currentSkin = 1;
        private const int maxSkins = 11; // Change to number of available skins

        public SkinPage()
        {
            InitializeComponent();
            Change_PacManSkin();
        }

        private void button_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow));
        }

        private void PacmanSkinForward_Click(object sender, RoutedEventArgs e)
        {
            currentSkin++;
            if (currentSkin > maxSkins)
                currentSkin = 1;
            Change_PacManSkin();
        }

        private void PacmanSkinBackwards_Click(object sender, RoutedEventArgs e)
        {
            currentSkin--;
            if (currentSkin < 1)
                currentSkin = maxSkins;
            Change_PacManSkin();
        }

        private void Change_PacManSkin()
        {
            ImageBrush brush = new ImageBrush();
            string uriString = $@"C:\Users\Mia Marjanovic\source\repos\PacmanWithoutMVVM\PacmanWithoutMVVM\Bilder\pacmanSkins\PacmanSkin{currentSkin}.png";
            brush.ImageSource = new BitmapImage(new Uri(uriString, UriKind.Absolute));
            PacmanSkinSlot.Fill = brush;

            // button ausgrauen
            if (MainWindow.Instance.ChosenSkin == currentSkin)
            {
                btn_SelectSkin.Background = new SolidColorBrush(Colors.Gray);
                btn_SelectSkin.IsEnabled = false;
            }
            else
            {
                btn_SelectSkin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFD100"));
                btn_SelectSkin.IsEnabled = true;
            }
        }

        private void btn_SelectSkin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ChosenSkin = currentSkin;
            Change_PacManSkin(); // Update the button appearance after selection
        }
    }
}
