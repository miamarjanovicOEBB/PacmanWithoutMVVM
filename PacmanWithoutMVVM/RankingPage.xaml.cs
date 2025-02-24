using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PacmanWithoutMVVM
{
    /// <summary>
    /// Interaction logic for RankingPage.xaml
    /// </summary>
    public partial class RankingPage : Page
    {
        // A static collection that holds all the game rounds
        private static ObservableCollection<RankingEntry> RankingList = new ObservableCollection<RankingEntry>();

        public RankingPage()
        {
            InitializeComponent();
            // Bind the ListView to the collection
            RankingListView.ItemsSource = RankingList;
        }

        // Class to store score and time
        public class RankingEntry
        {
            public string playername { get; set; }
            public int Score { get; set; }  // higher is better
            public int Time { get; set; }   // lower is better            

            public RankingEntry(int score, int time, string name)
            {
                Score = score;
                Time = time;
                playername = name;
            }
        }

        // Call this method from PlayPage when the game ends
        public static void AddToRanking(int score, int time, string name)
        {
            // Add new round result
            RankingList.Add(new RankingEntry(score, time, name));

            //sorted: neue, sotierte Liste
            //OrderByDescendig = Eine LINQ-Methode, die die Liste in absteigender Reihenfolge sortiert.
            // r => r.Score ist eine Lambda-Funktion, die für jedes Objekt r aus RankingList den Score zurückgibt.
            // Descending = absteigend
            var sorted = RankingList.OrderByDescending(r => r.Score) 
                                    .ThenBy(r => r.Time)
                                    .ToList();

            // Clear and re-add sorted entries (ObservableCollection updates the UI)
            RankingList.Clear();
            foreach (var entry in sorted)
            {
                RankingList.Add(entry);
            }

            // top 5 entries
            while (RankingList.Count > 5)
            {
                RankingList.RemoveAt(RankingList.Count - 1);
            }
        }

        private void btn_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new MainMenuePage(null));
        }
    }
}
