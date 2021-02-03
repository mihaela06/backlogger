using System.Data.Entity;
using System.Windows;
using Backlogger.Model;


namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            _ = System.IO.Directory.CreateDirectory(App.projectPath + @"\Resources\Images");

            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
            }
            InitializeComponent();

            DataContext = this;
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow("Books");
            this.Hide();
            collectionWindow.Show();
        }

        private void MoviesButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow("Movies");
            this.Hide();
            collectionWindow.Show();
        }

        private void GamesButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow("Games");
            this.Hide();
            collectionWindow.Show();
        }

        private void SuggestButton_Click(object sender, RoutedEventArgs e)
        {
            SuggestionsDialog suggestionsDialog = new SuggestionsDialog();
            suggestionsDialog.ShowDialog();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow();
            this.Hide();
            statisticsWindow.Show();
        }
    }
}
