using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Backlogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
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
    }
}
