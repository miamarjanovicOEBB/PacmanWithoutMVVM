using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using System.Windows.Threading;

namespace PacmanWithoutMVVM
{
    /// <summary>
    /// Interaktionslogik für PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page
    {
        private MainWindow mainWindow;
        private bool isPowerUpActive = false;

        DispatcherTimer gameTimer = new DispatcherTimer(); //pacmans bewegung
        DispatcherTimer showTime = new DispatcherTimer(); //Spielzeit anzeigen
        DispatcherTimer ghostTimer = new DispatcherTimer(); //Geister Bewegung
        DispatcherTimer powerUpTimer = new DispatcherTimer(); //dauer des Powerups        

        private (Rectangle ghost, double dx, double dy)[] ghosts; //array für Geisterobjekt und Richtungen
        private int secondsShown = 0; //Variable für die vergangenen Sekunde
        int score = 0; //Zähler für den Punktestand
        private bool isInvincible = false; // kurze ablaufzeit nachdem pacman vom geist getroffen wird
        int life = 3;
        double speed = 1;
        bool goUp, goDown, goLeft, goRight;
        bool noUp, noDown, noLeft, noRight;
        bool isPaused = false;
        private int ghostTouchCount = 0; //wie oft wird pacman vom geist berührt
        private string playername;

        private ImageBrush originalBlinkyImage, originalPinkyImage, originalInkyImage, originalClydeImage; //speichert die Originalbilder für Powerup
        Rect pacmanHitBox; 

        private void btn_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow)); //mainw
        }
        private void btn_pauseGame_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                gameTimer.Start();
                showTime.Start();
                ghostTimer.Start();
                powerUpTimer.Start();
                isPaused = false;
                btn_pauseGame.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFD100"));
            }
            else
            {
                gameTimer.Stop();
                showTime.Stop();
                ghostTimer.Stop();
                powerUpTimer.Stop();
                isPaused = true;              
                btn_pauseGame.Background = new SolidColorBrush(Colors.Gray);
            }
        }
        public PlayPage() //Konstruktor der PlayPage
        { 
            InitializeComponent(); //alle UI Elemente
            InitializeGhosts(); //geister und bewegung
            StartMoving(); //timer für geisterbewegung
            GameSetUp(); // Konfiguriert den Spiel-Update-Loop und andere Timer
            ApplySelectedSkin(); // Wendet den vom Spieler gewählten Pac-Man-Skin an
            PlayCanvas.Focus(); // Fokus auf das Spielfeld für Tastatureingaben

            powerUpTimer.Tick += EndPowerUp; // Event-Handler für das Ende des Power-Ups hinzufügen
        }
        private void InitializeGhosts()
        {
            // Speichern der Originalbilder der Geister
            originalBlinkyImage = Blinky.Fill as ImageBrush;
            originalPinkyImage = Pinky.Fill as ImageBrush;
            originalInkyImage = Inky.Fill as ImageBrush;
            originalClydeImage = Clyde.Fill as ImageBrush;

            ghosts = new (Rectangle, double, double)[] // Array mit Geistern und ihren initialen Bewegungsrichtungen
            {
                (Pinky, 1, 0),  // rechts
                (Inky, -0.6, 0), //links
                (Clyde, 0, 1),  // unten
                (Blinky, 0, -1)  // oben
            };
        }
        private void StartMoving()//ghost
        {
            ghostTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) }; //ca 50 fps
            ghostTimer.Tick += MoveGhosts; // Verknüpft den Timer mit dem MoveGhosts-Event-Handler
            ghostTimer.Start();
        }
        private void MoveGhosts(object sender, EventArgs e) // Aktualisiert die Positionen der Geister im Canvas
        {
            double canvasWidth = PlayCanvas.ActualWidth;
            double canvasHeight = PlayCanvas.ActualHeight;

            for (int i = 0; i < ghosts.Length; i++) //i stands for the loop index, goes through the array of ghosts
            {
                var (ghost, directionX, directionY) = ghosts[i]; //aktuellen Geist und seine Richtungswerte

                // Aktuelle Position berechnen
                double newX = Canvas.GetLeft(ghost) + directionX;
                double newY = Canvas.GetTop(ghost) + directionY;

                // Canvas-Grenzenprüfung
                if (newX < 0 || newX + ghost.Width > canvasWidth) // Überprüft Geist, linken oder rechten Rand
                {
                    directionX = -directionX; // Kehrt horizontale Bewegungsrichtung um
                }

                if (newY < 0 || newY + ghost.Height > canvasHeight) //oben und unten
                {
                    directionY = -directionY; // Umkehren, vertikale richtung
                }

                // prüft, ob an der neuen Position eine Kollision stattfindet
                if (CheckCollision(ghost, newX, newY))
                {
                    directionX = -directionX;
                    directionY = -directionY;
                }
                else // Falls keine Kollision -> Position des Geistes aktualisieren
                {
                    Canvas.SetLeft(ghost, newX);
                    Canvas.SetTop(ghost, newY);
                }

                // Aktualisierte Richtung speichern
                ghosts[i] = (ghost, directionX, directionY);
            }
        }
        private void ActivatePowerUp()
        {
            if (!isPowerUpActive)
            {
                isPowerUpActive = true;

                // Geister-Skins ändern auf den blauen Skin
                Blinky.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users\\Mia Marjanovic\\source\\repos\\PacmanWithoutMVVM\\PacmanWithoutMVVM\\Bilder\\GhostSkins\\ghostAngst.jpg", UriKind.Relative)) };
                Pinky.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users\\Mia Marjanovic\\source\\repos\\PacmanWithoutMVVM\\PacmanWithoutMVVM\\Bilder\\GhostSkins\\ghostAngst.jpg", UriKind.Relative)) };
                Inky.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users\\Mia Marjanovic\\source\\repos\\PacmanWithoutMVVM\\PacmanWithoutMVVM\\Bilder\\GhostSkins\\ghostAngst.jpg", UriKind.Relative)) };
                Clyde.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users\\Mia Marjanovic\\source\\repos\\PacmanWithoutMVVM\\PacmanWithoutMVVM\\Bilder\\GhostSkins\\ghostAngst.jpg", UriKind.Relative)) };

                powerUpTimer.Interval = TimeSpan.FromSeconds(3); // 3 Sekunden warten
                powerUpTimer.Start();
            }
        }
        private void EndPowerUp(object sender, EventArgs e)
        {
            // Geister zurücksetzen auf ihre ursprünglichen Skins
            Blinky.Fill = originalBlinkyImage;
            Pinky.Fill = originalPinkyImage;
            Inky.Fill = originalInkyImage;
            Clyde.Fill = originalClydeImage;

            isPowerUpActive = false;
            powerUpTimer.Stop(); // Timer anhalten
        }

        // Prüft, ob der Geist an der neuen Position (newX, newY) mit einer Wand kollidiert
        private bool CheckCollision(Rectangle ghost, double newX, double newY) 
        {
            Rect ghostRect = new Rect(newX, newY, ghost.Width, ghost.Height); //Hitbox geist

            foreach (UIElement element in PlayCanvas.Children) //geht alle Elemente am Canvas durch
            {
                if (element is Rectangle wall && (string)wall.Tag == "wall")
                {
                    // Ermittelt die Position und Größe der Wand
                    double wallX = Canvas.GetLeft(wall);
                    double wallY = Canvas.GetTop(wall);
                    Rect wallRect = new Rect(wallX, wallY, wall.Width, wall.Height);

                    if (ghostRect.IntersectsWith(wallRect)) // Kollision Geist Wand
                    {
                        return true; // Kollision erkannt
                    }
                }
            }

            return false; // Keine Kollision gefunden
        }
        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                if (!noLeft)
                {
                    goRight = goUp = goDown = false;
                    noRight = noUp = noDown = false;
                    goLeft = true;

                    //spiegelt 
                    pacman.RenderTransform = new ScaleTransform(-1, 1, pacman.Width / 2, pacman.Height / 2);
                }
            }

            if (e.Key == Key.D)
            {
                if (!noRight)
                {
                    noLeft = noUp = noDown = false;
                    goLeft = goUp = goDown = false;
                    goRight = true;

                    pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
                }
            }

            if (e.Key == Key.W)
            {
                if (!noUp)
                {
                    noRight = noDown = noLeft = false;
                    goRight = goDown = goLeft = false;
                    goUp = true;

                    pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
                }
            }

            if (e.Key == Key.S)
            {
                if (!noDown)
                {
                    noUp = noLeft = noRight = false;
                    goUp = goLeft = goRight = false;
                    goDown = true;

                    pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
                }
            }
        }
        private void GameSetUp() //Initialisiert den Spiel-Loop und weitere Timer
        {
            PlayCanvas.Focus();
            gameTimer.Tick += GameLoop; // Konfiguriert den gameTimer für den Spiel-Update-Loop (Bewegung, Kollisionsprüfung, ...)
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);  //ca 60 fps
            gameTimer.Start();


            showTime.Interval = TimeSpan.FromSeconds(1);
            showTime.Tick += UpdateTimerDisplay;
            showTime.Start();
        }
        private void UpdateTimerDisplay(object sender, EventArgs e)
        {
            secondsShown++;
            txtBlock_timer.Text = $"{secondsShown}"; // Aktualisiert den Timer-Text
        }

        // Haupt-Spiel-Loop, der in jedem Timer-Tick aufgerufen wird
        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = score;

            // Move Pac-Man speed = the amount he moves on canvas
            if (goRight)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }
            if (goUp)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }

            // Prevent Pacman from moving off the map 
            if (goDown && Canvas.GetTop(pacman) + 8 > PlayCanvas.Height) //nummern sind induviduel weil abstand vom rand
            {
                noDown = true;
                goDown = false;
            }

            if (goUp && Canvas.GetTop(pacman) - 3 < 0)
            {
                noUp = true;
                goUp = false;
            }

            if (goLeft && Canvas.GetLeft(pacman) - 3 < 0)
            {
                noLeft = true;
                goLeft = false;
            }

            if (goRight && Canvas.GetLeft(pacman) + pacman.Width + speed > PlayCanvas.Width)
            {
                noRight = true;
                goRight = false;
            }

            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width - 1, pacman.Height - 1); //-1 so it s easier to go through tide spaces

            foreach (var gameObjects in PlayCanvas.Children.OfType<Rectangle>()) //objects like wall and coins
            {
                Rect hitBox = new Rect(Canvas.GetLeft(gameObjects), Canvas.GetTop(gameObjects), gameObjects.Width, gameObjects.Height);

                // Wall collision detection
                if ((string)gameObjects.Tag == "wall")
                {
                    if (goLeft && pacmanHitBox.IntersectsWith(hitBox)) //links
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 2);
                        noLeft = true;
                        goLeft = false;
                    }

                    if (goRight && pacmanHitBox.IntersectsWith(hitBox)) //rechts
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 2);
                        noRight = true;
                        goRight = false;
                    }

                    if (goDown && pacmanHitBox.IntersectsWith(hitBox)) //unten
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 2);
                        noDown = true;
                        goDown = false;
                    }

                    if (goUp && pacmanHitBox.IntersectsWith(hitBox)) //oben
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 2);
                        noUp = true;
                        goUp = false;
                    }
                }

                if ((string)gameObjects.Tag == "powerup")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && gameObjects.Visibility == Visibility.Visible)
                    {
                        gameObjects.Visibility = Visibility.Hidden;
                        ActivatePowerUp();  // Power-Up aktivieren
                    }
                }

                // Coin collection
                if ((string)gameObjects.Tag == "coin")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && gameObjects.Visibility == Visibility.Visible)
                    {
                        gameObjects.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                if ((string)gameObjects.Tag == "ghost" && !isPowerUpActive && pacmanHitBox.IntersectsWith(hitBox) && !isInvincible)
                {
                    ghostTouchCount++; // Erhöhe die Anzahl der Berührungen mit einem Geist

                    life--;
                    txtbox_Life.Text = life.ToString();

                    if (life <= 0)
                    {
                        
                        GameOver("You got eaten");
                        MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow));
                    }
                    else
                    {
                        // Wenn Pac-Man noch Leben hat, Spiel zurückgesetzt
                        ResetGame();

                        // Nach 1 Sekunde wird Pac-Man wieder verwundbar
                        isInvincible = true;
                        Task.Delay(1000).ContinueWith(_ => isInvincible = false);
                    }

                    // Wenn Pac-Man 3-mal von einem Geist berührt wurde, zum Menü
                    if (ghostTouchCount >= 3)
                    {
                        MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow));
                    }
                }

                if ((string)gameObjects.Tag == "teleport")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        // Überprüfe, ob Pac-Man sich auf der linken Seite des Bildschirms befindet
                        if (Canvas.GetLeft(pacman) < 10)
                        {
                            // Teleportiere Pac-Man nach rechts
                            Canvas.SetLeft(pacman, 90);
                            goLeft= true; //weil es sonst spinnt und pacman stehen bleibt
                        }
                        else
                        {
                            // Teleportiere Pac-Man nach links
                            Canvas.SetLeft(pacman, 5);
                        }
                    }
                }
            } 

            if (score == 153)
            {
                GameOver("You Win, you collected all of the coins");
                MainWindow.Instance.ShowPage(new MainMenuePage(mainWindow));
            }
        }
        private void ResetGame()
        {
            // Setze Pac-Man und die Geister auf ihre Startpositionen zurück
            Canvas.SetLeft(pacman, 48); //Canvas.Left="48" Canvas.Top="73"
            Canvas.SetTop(pacman, 73);
            
            Canvas.SetLeft(Clyde, 21); // Canvas.Left="21" Canvas.Top="48"
            Canvas.SetTop(Clyde, 48);

            Canvas.SetLeft(Pinky, 48); //Canvas.Left="48" Canvas.Top="19"
            Canvas.SetTop(Pinky, 19);

            Canvas.SetLeft(Inky, 42); //Canvas.Left="42" Canvas.Top="47"
            Canvas.SetTop(Inky, 47);

            Canvas.SetLeft(Blinky, 75); //Canvas.Left="75" Canvas.Top="49"
            Canvas.SetTop(Blinky, 49);

            // Setze den Timer zurück
            gameTimer.Start();
            showTime.Start();
            ghostTimer.Start();
        }

        private void ApplySelectedSkin()
        {
            int skin = MainWindow.Instance.ChosenSkin;
            string uriString = $@"C:\Users\Mia Marjanovic\source\repos\PacmanWithoutMVVM\PacmanWithoutMVVM\Bilder\pacmanSkins\PacmanSkin{skin}.png";

            // Check if pacman element exists
            if (pacman != null)
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(uriString, UriKind.Absolute));
                pacman.Fill = brush; // Apply the skin
            }
        }
        private void GameOver(string message)
        {
            gameTimer.Stop();
            showTime.Stop();
            MessageBox.Show(message, "GRAH");
            RankingPage.AddToRanking(score, secondsShown, MainWindow.Instance.PlayerName);
        }
    }
}